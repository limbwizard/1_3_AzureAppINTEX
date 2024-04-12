    // Function to initialize feature columns to 0
    /*Dictionary<string, int> InitializeFeatureColumns()
    {
        var featureColumns = new Dictionary<string, int>();
        // Initializing for "Country of Transaction"
        featureColumns["country_of_transaction_India"] = 0;
        featureColumns["country_of_transaction_Russia"] = 0;
        featureColumns["country_of_transaction_USA"] = 0;
        featureColumns["country_of_transaction_United Kingdom"] = 0;

        // Initializing for "Shipping Address"
        featureColumns["shipping_address_India"] = 0;
        featureColumns["shipping_address_Russia"] = 0;
        featureColumns["shipping_address_USA"] = 0;
        featureColumns["shipping_address_United Kingdom"] = 0;

        // Initializing for "Bank"
        featureColumns["bank_HSBC"] = 0;
        featureColumns["bank_Halifax"] = 0;
        featureColumns["bank_Lloyds"] = 0;
        featureColumns["bank_Metro"] = 0;
        featureColumns["bank_Monzo"] = 0;
        featureColumns["bank_RBS"] = 0;

        // Initializing for "Type of Card"
        featureColumns["type_of_card_Visa"] = 0;

        // Set the appropriate feature based on the Order object
        SetFeatureFromMapping(features, countryMapping, Order.CountryOfTransaction);
        SetFeatureFromMapping(features, shippingAddressMapping, Model.ShippingAddress);
        SetFeatureFromMapping(features, bankMapping, Model.Bank);

        return featureColumns;
    }*/