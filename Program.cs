using System;
using System.Net.Mail;
using System.Text.RegularExpressions;

public static class EmailValidator
{
    /// <summary>
    /// Validates an email address for RFC 5322 compatibility, including several
    /// strange and less common formats.
    ///
    /// Note: The System.Net.Mail.MailAddress class provides a good level of RFC 5322
    /// compliance, but has some nuances:
    /// 1. Comments: It typically strips comments (e.g., "(comment)user@domain.com"
    ///    is treated as "user@domain.com"). This method will consider such addresses valid
    ///    if the address without comments is valid. If explicit validation of the
    ///    comment syntax itself is required, custom parsing would be necessary.
    /// 2. International Characters (EAI - RFC 6531/6532): While .NET's MailAddress
    ///    has improved support for Unicode in email addresses over time, full and
    ///    strict validation of all EAI edge cases might still be limited. For
    ///    critical EAI applications, consider dedicated EAI libraries or more
    ///    complex regex patterns that specifically handle Unicode character categories.
    /// 3. Display Names: This method focuses on the 'addr-spec' (the actual email address)
    ///    and does not validate the display name part (e.g., "John Doe" <john.doe@example.com>).
    /// </summary>
    /// <param name="emailAddress">The email address string to validate.</param>
    /// <returns>True if the email address is considered RFC 5322 compliant, otherwise false.</returns>
    public static bool IsValidRfc5322Email(string emailAddress)
    {
        System.Net.MailAddressParser 
        if (string.IsNullOrWhiteSpace(emailAddress))
        {
            return false;
        }

        try
        {
            // The MailAddress constructor performs significant RFC 5322 validation.
            // It will throw a FormatException if the address is malformed.
            // Note: It strips comments and focuses on the core address (addr-spec).
            var mailAddress = new MailAddress(emailAddress);

            // Optional: Further custom checks if MailAddress is not strict enough for specific requirements.
            // For example, if you need to specifically ensure comments are *not* present
            // or if you need to validate the syntax of comments themselves.

            // Example: To check if MailAddress actually stripped a comment,
            // you could compare the original string to mailAddress.Address, but
            // this goes beyond simply validating the core address structure.

            // For Internationalized Domain Names (IDN) or local parts with Unicode:
            // MailAddress generally supports IDN via Punycode conversion. For local parts,
            // it attempts to parse them. If it throws an exception, it's invalid.
            // If it parses, it's considered valid by MailAddress.

            return true;
        }
        catch (FormatException)
        {
            // The email address did not conform to the expected format.
            return false;
        }
        catch (Exception ex)
        {
            // Catch any other unexpected exceptions during validation.
            Console.WriteLine($"An unexpected error occurred during email validation: {ex.Message}");
            return false;
        }
    }

    public static void Main(string[] args)
    {
        Console.WriteLine("--- RFC 5322 Email Address Validation Tests ---");

        // Test Cases for Quoted Strings in the Local Part
        Console.WriteLine("\n--- Quoted Strings in Local Part ---");
        TestEmail("\"john smith\"@example.com");       // Valid (space allowed in quoted string)
        TestEmail("\"user..name\"@example.com");       // Valid (consecutive dots allowed in quoted string)
        TestEmail("\"test@test\"@example.com");        // Valid (@ symbol allowed in quoted string)
        TestEmail("\"\\\"\"@example.com");             // Valid (escaped characters)
        TestEmail("\"!#$%&'()*+,./:;<=>?@[\\]^_{|}~\"@example.com"); // Valid (many special chars in quoted string)

        // Test Cases for Comments in the Address
        Console.WriteLine("\n--- Comments in Address (MailAddress strips these) ---");
        TestEmail("user(comment)@example.com");         // Valid (comment stripped, user@example.com remains)
        TestEmail("user@example.com(comment)");         // Valid (comment stripped)
        TestEmail("(comment)user@example.com");         // Valid (comment stripped)
        TestEmail("user(comment)name(anothercomment)@example.com"); // Valid (comments stripped)

        // Test Cases for International Characters (RFC 6531/6532)
        Console.WriteLine("\n--- International Characters (EAI) ---");
        TestEmail("éléonore@example.com");              // Valid (diacritic) - MailAddress should handle
        TestEmail("δοκιμή@παράδειγμα.δοκιμή");          // Valid (Greek characters) - MailAddress support varies
        TestEmail("我買@屋企.香港");                  // Valid (Chinese characters) - MailAddress support varies

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

        Console.ReadLine();
    }

    private static void TestEmail(string email)
    {
        bool isValid = IsValidRfc5322Email(email);
        Console.WriteLine($"'{email,-40}' is {(isValid ? "VALID" : "INVALID")}");
    }
}