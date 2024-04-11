
#NUM2
import pandas as pd
import numpy as np
from sqlalchemy import create_engine
import urllib
from xgboost import XGBClassifier
from sklearn.model_selection import train_test_split
from sklearn.preprocessing import LabelEncoder, MinMaxScaler
from sklearn.metrics import classification_report, accuracy_score
import pickle

from skl2onnx.common.shape_calculator import calculate_linear_classifier_output_shapes
from onnxmltools.convert.xgboost.operator_converters.XGBoost import convert_xgboost
from skl2onnx import update_registered_converter

update_registered_converter(
    XGBClassifier,
    "XGBoostXGBClassifier",
    calculate_linear_classifier_output_shapes,
    convert_xgboost,
    options={
        "nocl": [True, False], 
        "zipmap": [True, False, "columns"],
        "raw_scores": [True, False],
        "num_class": [0, 2],
    },
)

#NUM3
# Construct the connection string for SQLAlchemy engine
params = urllib.parse.quote_plus(
    "Driver={ODBC Driver 18 for SQL Server};"
    "Server=tcp:azure-sqlserver-limb.database.windows.net,1433;"
    "Database=AzureDBINTEX;"
    "Uid=limb06;"
    "Pwd=1_3_INTEX2;"
    "Encrypt=yes;"
    "TrustServerCertificate=no;"
    "Connection Timeout=30;"
)

conn_str = f'mssql+pyodbc:///?odbc_connect={params}'
engine = create_engine(conn_str)

#NUM4
# Optimized SQL query to fetch only the required columns
query = '''
    SELECT o.TransactionID, o.UserId, o.Date, o.DayOfWeek, o.Time, o.EntryMode, o.Amount, o.TypeOfTransaction,
           o.CountryOfTransaction, o.ShippingAddress, o.Bank, o.TypeOfCard,
           CASE WHEN o.Fraud IS NULL THEN 0 ELSE o.Fraud END AS Fraud
    FROM Orders o
'''
df = pd.read_sql(query, engine)

# Display basic information about the fetched data
print(f"Shape of the dataset: {df.shape}")
print("First few rows of the dataset:")
print(df.head())

#NUM5
# Data Preparation
print("Shape of the dataset before dropping missing values:", df.shape)
df.dropna(inplace=True)
print("Shape of the dataset after dropping missing values:", df.shape)


# NUM6: Enhanced Preprocessing of the Data

import numpy as np
from sklearn.preprocessing import MinMaxScaler

# Ensure 'Date' is in datetime format and calculate the number of days since a reference date
df['Date'] = pd.to_datetime(df['Date'])
df['DaysSinceReference'] = (df['Date'] - pd.to_datetime('2023-01-01')).dt.days
df.drop('Date', axis=1, inplace=True)  # Drop the 'Date' column after converting it

# Map 'DayOfWeek' to numerical values
day_mapping = {'Sunday': 0, 'Monday': 1, 'Tuesday': 2, 'Wednesday': 3, 'Thursday': 4, 'Friday': 5, 'Saturday': 6}
df['DayOfWeek'] = df['DayOfWeek'].map(day_mapping)

# Binning categorical features based on frequency
def bin_categories(df, features=[], cutoff=0.02, replace_with='Other'):
    for feat in features:
        if not pd.api.types.is_numeric_dtype(df[feat]):
            frequencies = df[feat].value_counts(normalize=True)  # Get frequency as a percentage
            other = frequencies[frequencies < cutoff].index  # Find categories with freq less than cutoff
            df[feat] = df[feat].replace(other, replace_with)  # Replace those categories with 'Other'
    return df

# Define categorical features to bin and one-hot encode
categorical_features = ['EntryMode', 'TypeOfTransaction', 'CountryOfTransaction', 'Bank', 'TypeOfCard']
df = bin_categories(df, features=categorical_features)

# One-hot encode categorical features
df = pd.get_dummies(df, columns=categorical_features, drop_first=True)

# Scale numeric features - 'Amount'
scaler = MinMaxScaler()
df['Amount'] = scaler.fit_transform(df[['Amount']])

# Apply the sin and cos transformations for cyclical nature
df['Time_sin'] = np.sin(2 * np.pi * df['Time']/1440)
df['Time_cos'] = np.cos(2 * np.pi * df['Time']/1440)
df.drop('Time', axis=1, inplace=True)  # Drop original Time column after transformation

# Show the DataFrame head to verify the changes
print(df.head())


#NUM7
# Get the categorical feature names from the updated DataFrame
categorical_features = df.drop(['Fraud', 'UserId', 'ShippingAddress'], axis=1).select_dtypes(include=['object']).columns.tolist()

# One-hot encode the categorical features
new_features = pd.get_dummies(df.drop(['Fraud', 'UserId', 'ShippingAddress'], axis=1), columns=categorical_features)

# Create a new DataFrame with feature names following the required pattern
new_feature_names = [f'f{i}' for i in range(new_features.shape[1])]
new_features = new_features.rename(columns=dict(zip(new_features.columns, new_feature_names)))

# Split the data into features and target
target = df.Fraud
features = new_features

#NUM8
# Encode the target variable
le = LabelEncoder()
le.fit(target)
target_encoded = le.transform(target)


#NUM9
# Split the data into training and testing sets
X_train, X_test, y_train, y_test = train_test_split(features, target_encoded, test_size=0.3, random_state=1)


# NUM10: Model Training with Optimal Parameters
from xgboost import XGBClassifier

# Specify the optimal parameters identified from previous runs
optimal_params = {
    'colsample_bytree': 1.0,
    'learning_rate': 0.1,
    'max_depth': 2,
    'n_estimators': 50,
    'subsample': 0.8,
    'use_label_encoder': False,  # To avoid warnings since we're using an encoded target variable
    'eval_metric': 'logloss',    # Choosing 'logloss' for binary classification
    'enable_categorical': True   # Assuming categorical variables are encoded as such
}

# Initialize XGBClassifier with the optimal parameters
xgb_model = XGBClassifier(**optimal_params)

# Training the model on the training dataset
xgb_model.fit(X_train, y_train)


# NUM11: Making Predictions and Evaluating the Model
from sklearn.metrics import classification_report, accuracy_score

# Use the trained model to make predictions on the test set
y_pred = xgb_model.predict(X_test)
y_pred_reversed = le.inverse_transform(y_pred)
y_test_reversed = le.inverse_transform(y_test)

# Evaluate the model's performance
print("Classification Report:")
print(classification_report(y_test_reversed, y_pred_reversed))
print(f"Accuracy: {accuracy_score(y_test, y_pred)}")


# NUM12: Model Evaluation with ROC Curve and AUC
from sklearn.metrics import roc_auc_score, roc_curve, auc
import matplotlib.pyplot as plt

# Calculate the ROC AUC score
y_pred_proba = xgb_model.predict_proba(X_test)[:, 1]
roc_auc = roc_auc_score(y_test, y_pred_proba)

fpr, tpr, thresholds = roc_curve(y_test, y_pred_proba)
plt.figure()
plt.plot(fpr, tpr, label='ROC curve (area = %0.2f)' % roc_auc)
plt.plot([0, 1], [0, 1], 'k--')
plt.xlim([0.0, 1.0])
plt.ylim([0.0, 1.05])
plt.xlabel('False Positive Rate')
plt.ylabel('True Positive Rate')
plt.title('Receiver Operating Characteristic')
plt.legend(loc="lower right")
plt.show()

# Print classification report
print("Classification Report:")
print(classification_report(y_test_reversed, y_pred_reversed))


# NUM13: Finalizing Model Serialization with Pickle
import pickle

# Save the model to a file
filename = 'finalized_model.sav'
with open(filename, 'wb') as file:
    pickle.dump(xgb_model, file)

print(f"Model saved to {filename}")


from skl2onnx import convert_sklearn
from skl2onnx.common.data_types import FloatTensorType

initial_type = [('float_input', FloatTensorType([None, X_train.shape[1]]))]
onnx_model = convert_sklearn(xgb_model, initial_types=initial_type, target_opset=11)  # Experiment with the opset version

with open("finalized_xgb_model.onnx", "wb") as f:
    f.write(onnx_model.SerializeToString())


'''
# NUM10: Advanced Model Training with Expanded Grid Search (COMMENTED OUT)
# This section can be uncommented if you want to perform a grid search for hyperparameter tuning
# before training the final model.

# from xgboost import XGBClassifier
# from sklearn.model_selection import GridSearchCV

# # Before model training, ensure no datetime64[ns] types or other non-compatible types remain.
# df = df.select_dtypes(exclude=['datetime', 'object'])

# # Assuming 'f1' is your datetime column, here's a simple conversion to a numerical feature, such as extracting the day of year
# if 'f1' in X_train.columns:
#     X_train['f1_day_of_year'] = X_train['f1'].dt.dayofyear
#     X_train.drop(columns=['f1'], inplace=True)

# if 'f1' in X_test.columns:
#     X_test['f1_day_of_year'] = X_test['f1'].dt.dayofyear
#     X_test.drop(columns=['f1'], inplace=True)


# # Also, double-check that 'enable_categorical' is set to True if you have categorical variables encoded numerically.
# xgb = XGBClassifier(random_state=1, use_label_encoder=False, eval_metric='logloss', enable_categorical=True)

# param_grid = {
#     'n_estimators': [100, 200],
#     'max_depth': [3, 5, 7],
#     'learning_rate': [0.01, 0.1],
#     'subsample': [0.8, 1.0],
#     'colsample_bytree': [0.8, 1.0],
# }

# grid_search = GridSearchCV(estimator=xgb, param_grid=param_grid, cv=5, scoring='roc_auc', verbose=1, n_jobs=-1)
# grid_search.fit(X_train, y_train)

# print("Best parameters found: ", grid_search.best_params_)
'''