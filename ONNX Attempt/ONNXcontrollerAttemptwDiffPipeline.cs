/*public IActionResult ShowPredictions()
{
    var records = _context.Orders
            .OrderByDescending(o => o.Date)
            .Take(20)
            .ToList();
    var predictions = new List<FraudPrediction>();

    // Dictionary mapping the numeric prediction to an animal type
    var fraud_or_nah = new Dictionary<int, string>
        {
            { 0, "Not Fraud" },
            { 1, "Fraud" }

        };


    // Define a mapping for each categorical feature **********************************************************
    var entryModeMapping = new Dictionary<string, string>
    {
        {"PIN", "entry_mode_PIN"},
        {"Tap", "entry_mode_Tap" }
    };

    var typeOfTransactionMapping = new Dictionary<string, string>
    {
        {"OnLine", "type_of_transaction_Online"},
        {"POS", "type_of_transaction_POS" }
    };
    var countryMapping = new Dictionary<string, string>
    {
        { "India", "country_of_transaction_India" },
        { "Russia", "country_of_transaction_Russia" },
        {"USA", "country_of_transaction_USA"},
        {"United Kingdom", "country_of_transaction_United Kingdom"}
    };

    var shippingAddressMapping = new Dictionary<string, string>
    {
        { "India", "shipping_address_India" },
        { "Russia", "shipping_address_Russia" },
        { "USA", "shipping_address_USA" },
        { "United Kingdom", "shipping_address_United Kingdom" },
    };

    var bankMapping = new Dictionary<string, string>
    {
        { "HSBC", "bank_HSBC" },
        { "Halifax", "bank_Halifax" },
        { "Lloyds", "bank_Lloyds" },
        { "Metro", "bank_Metro" },
        { "Monzo", "bank_Monzo" },
        { "RBS", "bank_RBS" },
    };

    var typeOfCardMapping = new Dictionary<string, string>
    {
        { "Visa", "type_of_card_Visa" }

    };

    // Generic function to set a single feature based on a given mapping and value
    void SetFeatureFromMapping(Dictionary<string, int> features, Dictionary<string, string> mapping, string value)
    {
        if (!string.IsNullOrEmpty(value) && mapping.TryGetValue(value, out var featureName))
        {
            features[featureName] = 1;
        }
    }

    Dictionary<string, int> InitializeFeatureColumns()
    {
        var featureColumns = new Dictionary<string, int>
            {
                // Initialize one-hot encoded categorical features
                {"entry_mode_PIN", 0},
                {"entry_mode_Tap", 0},

                {"type_of_transaction_Online", 0},
                {"type_of_transaction_POS", 0},

                {"country_of_transaction_India", 0},
                {"country_of_transaction_Russia", 0},
                {"country_of_transaction_USA", 0},
                {"country_of_transaction_United Kingdom", 0},

                {"shipping_address_India", 0},
                {"shipping_address_Russia", 0},
                {"shipping_address_USA", 0},
                {"shipping_address_United Kingdom", 0},

                {"bank_HSBC", 0},
                {"bank_Halifax", 0},
                {"bank_Lloyds", 0 },
                {"bank_Metro", 0 },
                {"bank_Monzo", 0 },
                {"bank_RBS", 0 },

                {"type_of_card_Visa", 0}
            };

        return featureColumns;

    }


    //*************************************************************************************************************

    foreach (var record in records)
    {
        var jan1 = new DateTime(2023, 1, 1);
        var daysSinceJan1 = record.Date.HasValue ? Math.Abs((record.Date.Value - jan1).Days) : 0;

        // Initialize dummy-encoded feature columns to 0
        var features = InitializeFeatureColumns();

        // Apply mappings to set appropriate dummy-encoded features
        SetFeatureFromMapping(features, entryModeMapping, record.EntryMode);
        SetFeatureFromMapping(features, typeOfTransactionMapping, record.TypeOfTransaction);
        SetFeatureFromMapping(features, countryMapping, record.CountryOfTransaction);
        SetFeatureFromMapping(features, shippingAddressMapping, record.ShippingAddress);
        SetFeatureFromMapping(features, bankMapping, record.Bank);
        SetFeatureFromMapping(features, typeOfCardMapping, record.TypeOfCard);


        int dayOfWeekValue;
        if (record.DayOfWeek == "Mon")
        {
            dayOfWeekValue = 0;
        }
        else if (record.DayOfWeek == "Tues")
        {
            dayOfWeekValue = 1;
        }
        else if (record.DayOfWeek == "Wed")
        {
            dayOfWeekValue = 2;
        }
        else if (record.DayOfWeek == "Thu")
        {
            dayOfWeekValue = 3;
        }
        else if (record.DayOfWeek == "Fri")
        {
            dayOfWeekValue = 4;
        }
        else if (record.DayOfWeek == "Sat")
        {
            dayOfWeekValue = 5;
        }
        else if (record.DayOfWeek == "Sun")
        {
            dayOfWeekValue = 6;
        }
            else
        {
            dayOfWeekValue = 0; // Or some other default value
        }

            var input = new List<float>
        {


            record.TransactionID,
            float.Parse(record.UserId),
            daysSinceJan1,
            dayOfWeekValue,
            (float)record.Time,
            (float)record.Amount,
            features.ContainsKey("entry_mode_PIN") ? features["entry_mode_PIN"] : 0,
            features.ContainsKey("entry_mode_Tap") ? features["entry_mode_Tap"] : 0,

            features.ContainsKey("type_of_transaction_Online") ? features["type_of_transaction_Online"] : 0,
            features.ContainsKey("type_of_transaction_POS") ? features["type_of_transaction_POS"] : 0,

            features.ContainsKey("country_of_transaction_India") ? features["country_of_transaction_India"] : 0,
            features.ContainsKey("country_of_transaction_Russia") ? features["country_of_transaction_Russia"] : 0,
            features.ContainsKey("country_of_transaction_USA") ? features["country_of_transaction_USA"] : 0,
            features.ContainsKey("country_of_transaction_United Kingdom") ? features["country_of_transaction_United Kingdom"] : 0,

            features.ContainsKey("shipping_address_India") ? features["shipping_address_India"] : 0,
            features.ContainsKey("shipping_address_Russia") ? features["shipping_address_Russia"] : 0,
            features.ContainsKey("shipping_address_USA") ? features["shipping_address_USA"] : 0,
            features.ContainsKey("shipping_address_United Kingdom") ? features["shipping_address_United Kingdom"] : 0,

            features.ContainsKey("bank_HSBC") ? features["bank_HSBC"] : 0,
            features.ContainsKey("bank_Halifax") ? features["bank_Halifax"] : 0,
            features.ContainsKey("bank_Lloyds") ? features["bank_Lloyds"] : 0,
            features.ContainsKey("bank_Metro") ? features["bank_Metro"] : 0,
            features.ContainsKey("bank_Monzo") ? features["bank_Monzo"] : 0,
            features.ContainsKey("bank_RBS") ? features["bank_RBS"] : 0,

            features.ContainsKey("type_of_card_Visa") ? features["type_of_card_Visa"] : 0


        };

        *//*            ['transaction_ID', 'customer_ID', 'date', 'day_of_week', 'time', 'amount', 'entry_mode_PIN', 'entry_mode_Tap', 
                        'type_of_transaction_Online', 'type_of_transaction_POS', 'country_of_transaction_India', 
                        'country_of_transaction_Russia', 'country_of_transaction_USA', 'country_of_transaction_United Kingdom',
                        'shipping_address_India', 'shipping_address_Russia', 'shipping_address_USA',
                        'shipping_address_United Kingdom', 'bank_HSBC', 'bank_Halifax', 'bank_Lloyds',
                        'bank_Metro', 'bank_Monzo', 'bank_RBS', 'type_of_card_Visa']*//*



        var inputTensor = new DenseTensor<float>(input.ToArray(), new[] { 1, input.Count });

        var inputs = new List<NamedOnnxValue>
        {
        NamedOnnxValue.CreateFromTensor("float_input", inputTensor)
        };

        string predictionResult;
        using (var results = _session.Run(inputs))
        {
            var prediction = results.FirstOrDefault(item => item.Name == "output_label")?.AsTensor<long>().ToArray();
            predictionResult = prediction != null && prediction.Length > 0 ? fraud_or_nah.GetValueOrDefault((int)prediction[0], "Unknown") : "Error in prediction";

        }

        predictions.Add(new FraudPrediction { Order = record, Prediction = predictionResult }); // Adds the animal information and prediction for that animal to AnimalPrediction viewmodel

    }


    return View("ViewOrders", predictions);
}*/