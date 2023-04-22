namespace QueryKit.UnitTests;

using FluentAssertions;
using WebApiTestProject.Entities;

public class FilterParserTests
{
    [Fact]
    public void escaped_double_quote_with_more_than_3_double_quotes()
    {
        var input = """""""""Title == """"lamb is great on a "gee-ro" not a "gy-ro" sandwich"""" """"""""";
        var temp = """"Title = ""gyro"" """";
        var filterExpression = FilterParser.ParseFilter<TestingPerson>(input);
        var asString = filterExpression.ToString();
        asString.Should()
            .Be(""""x => (x.Title == "lamb is great on a \"gee-ro\" not a \"gy-ro\" sandwich")"""");
    }
    
    [Fact]
    public void escaped_double_quote()
    {
        var input = """""Title == """lamb is great on a "gee-ro" not a "gy-ro" sandwich""" """"";

        var filterExpression = FilterParser.ParseFilter<TestingPerson>(input);
        var asString = filterExpression.ToString();
        asString.Should()
            .Be(""""x => (x.Title == "lamb is great on a \"gee-ro\" not a \"gy-ro\" sandwich")"""");
    }
    
    [Fact]
    public void complex_with_lots_of_types()
    {
        var input =
            """""((Title @=* "waffle & chicken" && Age > 30) || Id == "aa648248-cb69-4217-ac95-d7484795afb2" || Title == "lamb" || Title == null) && (Age < 18 || (BirthMonth == "January" && Title _= "ally")) || Rating > 3.5 || SpecificDate == 2022-07-01T00:00:03Z && (Date == 2022-07-01 || Time == 00:00:03)""""";

        var filterExpression = FilterParser.ParseFilter<TestingPerson>(input);
        filterExpression.ToString().Should()
            .Be(""""x => (((((((x.Title.ToLower().Contains("waffle & chicken".ToLower()) AndAlso (x.Age > 30)) OrElse (x.Id == Parse("aa648248-cb69-4217-ac95-d7484795afb2"))) OrElse (x.Title == "lamb")) OrElse (x.Title == null)) AndAlso ((x.Age < 18) OrElse ((x.BirthMonth == "January") AndAlso x.Title.StartsWith("ally")))) OrElse (x.Rating > 3.5)) OrElse ((x.SpecificDate == 7/1/2022 12:00:03 AM +00:00) AndAlso ((x.Date == 7/1/2022) OrElse (x.Time == 12:00 AM))))"""");
    }
    
    [Fact]
    public void order_of_ops_quote_on_string()
    {
        var input = """(Title @=* "waffle" || Age > 30) || Age < 18""";
        var filterExpression = FilterParser.ParseFilter<TestingPerson>(input);
        filterExpression.ToString().Should()
            .Be(""""x => ((x.Title.ToLower().Contains("waffle".ToLower()) OrElse (x.Age > 30)) OrElse (x.Age < 18))"""");
    }
    
    [Fact]
    public void simple_string()
    {
        var input = """"Title @=* "waffle" """";
        var filterExpression = FilterParser.ParseFilter<TestingPerson>(input);
        filterExpression.ToString().Should().Be(""""x => x.Title.ToLower().Contains("waffle".ToLower())"""");
    }
    
    [Fact]
    public void can_handle_null()
    {
        var input = "Title == null";
        var filterExpression = FilterParser.ParseFilter<TestingPerson>(input);
        filterExpression.ToString().Should().Be("x => (x.Title == null)");
    }
    
    [Fact]
    public void can_handle_guid_with_double_quotes()
    {
        var guid = Guid.NewGuid();
        var input = $"""Id == "{guid}" """;
        var filterExpression = FilterParser.ParseFilter<TestingPerson>(input);
        filterExpression.ToString().Should().Be($"x => (x.Id == Parse(\"{guid}\"))");
    }
    
    [Fact]
    public void can_handle_guid_without_double_quotes()
    {
        var guid = Guid.NewGuid();
        var input = $"""Id == {guid} """;
        var filterExpression = FilterParser.ParseFilter<TestingPerson>(input);
        filterExpression.ToString().Should().Be($"x => (x.Id == Parse(\"{guid}\"))");
    }
    
    [Fact]
    public void equality_operator()
    {
        var input = """Title == "lamb" """;
        var filterExpression = FilterParser.ParseFilter<TestingPerson>(input);
        filterExpression.ToString().Should().Be("x => (x.Title == \"lamb\")");
    }

    [Fact]
    public void inequality_operator()
    {
        var input = """Title != "lamb" """;
        var filterExpression = FilterParser.ParseFilter<TestingPerson>(input);
        filterExpression.ToString().Should().Be("x => (x.Title != \"lamb\")");
    }

    [Fact]
    public void greater_than_operator()
    {
        var input = """Age > 30""";
        var filterExpression = FilterParser.ParseFilter<TestingPerson>(input);
        filterExpression.ToString().Should().Be("x => (x.Age > 30)");
    }

    [Fact]
    public void greater_than_or_equal_to_operator()
    {
        var input = """Age >= 30""";
        var filterExpression = FilterParser.ParseFilter<TestingPerson>(input);
        filterExpression.ToString().Should().Be("x => (x.Age >= 30)");
    }

    [Fact]
    public void less_than_operator()
    {
        var input = """Age < 30""";
        var filterExpression = FilterParser.ParseFilter<TestingPerson>(input);
        filterExpression.ToString().Should().Be("x => (x.Age < 30)");
    }

    [Fact]
    public void less_than_or_equal_to_operator()
    {
        var input = """Age <= 30""";
        var filterExpression = FilterParser.ParseFilter<TestingPerson>(input);
        filterExpression.ToString().Should().Be("x => (x.Age <= 30)");
    }

    [Fact]
    public void and_operator()
    {
        var input = """Title == "lamb" && Age > 30""";
        var filterExpression = FilterParser.ParseFilter<TestingPerson>(input);
        filterExpression.ToString().Should().Be("x => ((x.Title == \"lamb\") AndAlso (x.Age > 30))");
    }

    [Fact]
    public void or_operator()
    {
        var input = """Title == "lamb" || Age > 30""";
        var filterExpression = FilterParser.ParseFilter<TestingPerson>(input);
        filterExpression.ToString().Should().Be("x => ((x.Title == \"lamb\") OrElse (x.Age > 30))");
    }

    [Fact]
    public void contains_operator()
    {
        var input = """Title @=* "waffle" """;
        var filterExpression = FilterParser.ParseFilter<TestingPerson>(input);
        filterExpression.ToString().Should().Be("x => x.Title.ToLower().Contains(\"waffle\".ToLower())");
    }

    [Fact]
    public void starts_with_operator()
    {
        var input = """Title _= "lam" """;
        var filterExpression = FilterParser.ParseFilter<TestingPerson>(input);
        filterExpression.ToString().Should().Be("x => x.Title.StartsWith(\"lam\")");
    }

    [Fact]
    public void ends_with_operator_case_insensitive()
    {
        var input = """Title _-=* "b" """;
        var filterExpression = FilterParser.ParseFilter<TestingPerson>(input);
        filterExpression.ToString().Should().Be("x => x.Title.ToLower().EndsWith(\"b\".ToLower())");
    }

    [Fact]
    public void ends_with_operator()
    {
        var input = """Title _-= "b" """;
        var filterExpression = FilterParser.ParseFilter<TestingPerson>(input);
        filterExpression.ToString().Should().Be("x => x.Title.EndsWith(\"b\")");
    }
    
    [Fact]
    public void test_logical_operators()
    {
        var input = """(Age == 35) && (Favorite == true) || (Age < 18)""";
        var filterExpression = FilterParser.ParseFilter<TestingPerson>(input);
        filterExpression.ToString().Should()
            .Be(""""x => (((x.Age == 35) AndAlso (x.Favorite == True)) OrElse (x.Age < 18))"""");
    }
    
    [Fact]
    public void can_handle_case_insensitive_props()
    {
        var input = """(age == 35) && (favorite == true) || (age < 18)""";
        var filterExpression = FilterParser.ParseFilter<TestingPerson>(input);
        filterExpression.ToString().Should()
            .Be(""""x => (((x.Age == 35) AndAlso (x.Favorite == True)) OrElse (x.Age < 18))"""");
    }

    [Fact]
    public void can_handle_parentheses_and_logical_operators()
    {
        var input = """(Title == "lamb") && (Age > 30) || (Title == "chicken") && (Age < 18)""";
        var filterExpression = FilterParser.ParseFilter<TestingPerson>(input);
        filterExpression.ToString().Should().Be("x => (((x.Title == \"lamb\") AndAlso (x.Age > 30)) OrElse ((x.Title == \"chicken\") AndAlso (x.Age < 18)))");
    }
    
    [Fact]
    public void can_handle_decimal_comparison()
    {
        var input = """Rating >= 3.5""";
        var filterExpression = FilterParser.ParseFilter<TestingPerson>(input);
        filterExpression.ToString().Should().Be("x => (x.Rating >= 3.5)");
    }

    [Fact]
    public void can_handle_datetime_comparison()
    {
        var input = """SpecificDate == 2022-07-01T00:00:03Z""";
        var filterExpression = FilterParser.ParseFilter<TestingPerson>(input);
        filterExpression.ToString().Should().Be("x => (x.SpecificDate == 7/1/2022 12:00:03 AM +00:00)");
    }
    
    [Fact]
    public void can_handle_childproperty()
    {
        var input = """Email.Value == "john@example.com" """;
        var filterExpression = FilterParser.ParseFilter<TestingPerson>(input);
        filterExpression.ToString().Should().Be("x => (x.Email.Value == \"john@example.com\")");
    }

    [Fact]
    public void can_handle_childproperty_contains()
    {
        var input = """Email.Value @=* "example" """;
        var filterExpression = FilterParser.ParseFilter<TestingPerson>(input);
        filterExpression.ToString().Should().Be("x => x.Email.Value.ToLower().Contains(\"example\".ToLower())");
    }

    [Fact]
    public void can_handle_time_comparison()
    {
        var input = """Time == 00:00:03""";
        var filterExpression = FilterParser.ParseFilter<TestingPerson>(input);
        filterExpression.ToString().Should().Be("x => (x.Time == 12:00 AM)");
    }

    [Fact]
    public void multiple_properties_and_operators()
    {
        var input = """Title _= "lamb" && Age >= 25 && Rating < 4.5 && SpecificDate <= 2022-07-01T00:00:03Z && Time == 00:00:03""";
        var filterExpression = FilterParser.ParseFilter<TestingPerson>(input);
        filterExpression.ToString().Should().Be("x => ((((x.Title.StartsWith(\"lamb\") AndAlso (x.Age >= 25)) AndAlso (x.Rating < 4.5)) AndAlso (x.SpecificDate <= 7/1/2022 12:00:03 AM +00:00)) AndAlso (x.Time == 12:00 AM))");
    }

    [Fact]
    public void complex_filter_with_nested_parentheses()
    {
        var input = """(Title == "lamb" && ((Age >= 25 && Rating < 4.5) || (SpecificDate <= 2022-07-01T00:00:03Z && Time == 00:00:03)) && (Favorite == true || Email.Value _= "example"))""";
        var filterExpression = FilterParser.ParseFilter<TestingPerson>(input);
        filterExpression.ToString().Should().Be("x => (((x.Title == \"lamb\") AndAlso (((x.Age >= 25) AndAlso (x.Rating < 4.5)) OrElse ((x.SpecificDate <= 7/1/2022 12:00:03 AM +00:00) AndAlso (x.Time == 12:00 AM)))) AndAlso ((x.Favorite == True) OrElse x.Email.Value.StartsWith(\"example\")))");
    }

    [Fact]
    public void can_handle_null_childproperty()
    {
        var input = """Email.Value == null""";
        var filterExpression = FilterParser.ParseFilter<TestingPerson>(input);
        filterExpression.ToString().Should().Be("x => (x.Email.Value == null)");
    }

}