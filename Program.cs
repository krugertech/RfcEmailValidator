using System;
using QuickDevTest;

Console.WriteLine("--- RFC 5322 Email Address Validation Tests ---");

// Test Cases for Quoted Strings in the Local Part
Console.WriteLine("\n--- Quoted Strings in Local Part ---");
TestEmail("\"john smith\"@example.co.za");       // Valid (space allowed in quoted string)
TestEmail("\"user..name\"@example.com");       // Valid (consecutive dots allowed in quoted string)
TestEmail("\"test@test\"@example.com");        // Valid (@ symbol allowed in quoted string)
TestEmail("\"\\\"\"@example.com");             // Valid (escaped characters)
TestEmail("\"!#$%&'()*+,./:;<=>?@[\\]^_{|}~\"@example.com"); // Valid (many special chars in quoted string)

// Test Cases for Comments in the Address
Console.WriteLine("\n--- Comments in Address (These should be handled) ---");
TestEmail("user(comment)@example.com");         // Valid (comment stripped, user@example.com remains)
TestEmail("user@example.com(comment)");         // Valid (comment stripped)
TestEmail("(comment)user@example.com");         // Valid (comment stripped)
TestEmail("user(comment)name(anothercomment)@example.com"); // Valid (comments stripped)

// Test Cases for International Characters (RFC 6531/6532)
Console.WriteLine("\n--- International Characters (EAI) ---");
TestEmail("éléonore@example.com");              // Valid (diacritic)
TestEmail("user@münchen.de");                   // Valid (international domain)

// Test Cases for Addresses with IP Literals
Console.WriteLine("\n--- IP Literals in Domain ---");
TestEmail("user@[192.168.2.1]");               // Valid (IPv4 literal)
TestEmail("user@[IPv6:2001:db8::1]");          // Valid (IPv6 literal)

// Other RFC-compliant but unusual cases
Console.WriteLine("\n--- Other RFC-compliant unusual cases ---");
TestEmail("!name@place.com");                  // Valid (exclamation mark at start of local part)
TestEmail("user.name+tag@example.com");        // Valid (plus addressing)
TestEmail("user-name@example.com");            // Valid (hyphen)
TestEmail("user_name@example.com");            // Valid (underscore)

// Test display name parsing
Console.WriteLine("\n--- Display Name Parsing ---");
TestEmailWithParsing("\"John Doe\" <john.doe@example.com>");
TestEmailWithParsing("John Doe <john.doe@example.com>");
TestEmailWithParsing("john.doe@example.com");

// Invalid examples for comparison
Console.WriteLine("\n--- Invalid Examples ---");
TestEmail("invalid-email");                     // Invalid (missing @ and domain)
TestEmail("user@.com");                         // Invalid (domain starts with dot)
TestEmail("user@domain");                       // Invalid (missing TLD)
TestEmail("user@domain..com");                  // Invalid (consecutive dots in domain)
TestEmail("user@domain.com.");                  // Invalid (domain ends with dot)
TestEmail(".user@domain.com");                  // Invalid (local part starts with dot)
TestEmail("user..name@domain.com");             // Invalid (consecutive dots in unquoted local part)
TestEmail("user name@domain.com");              // Invalid (space in unquoted local part)

Console.WriteLine("\nPress any key to exit...");
Console.ReadKey();

static void TestEmail(string email)
{
    bool isValid = EmailValidator.IsValidEmail(email);
    Console.WriteLine($"'{email,-40}' is {(isValid ? "VALID" : "INVALID")}");
}

static void TestEmailWithParsing(string email)
{
    bool isValid = EmailValidator.IsValidEmail(email);
    var parsed = EmailValidator.ParseEmail(email);
    
    Console.WriteLine($"'{email,-40}' is {(isValid ? "VALID" : "INVALID")}");
    
    if (parsed != null)
    {
        Console.WriteLine($"  Display Name: '{parsed.DisplayName}'");
        Console.WriteLine($"  Local Part: '{parsed.LocalPart}'");
        Console.WriteLine($"  Domain: '{parsed.Domain}'");
        Console.WriteLine($"  Full Address: '{parsed.Address}'");
    }
    Console.WriteLine();
}