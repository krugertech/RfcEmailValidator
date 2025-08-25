using System;
using System.Linq;
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

// Comprehensive normal email address variations
Console.WriteLine("\n--- Normal Email Address Variations ---");

// Basic formats
Console.WriteLine("\n  Basic Formats:");
TestEmail("user@example.com");                  // Standard format
TestEmail("test@test.co.uk");                   // UK domain
TestEmail("admin@site.co.za");                  // South African domain
TestEmail("info@company.org");                  // Organization domain
TestEmail("support@service.net");               // Network domain
TestEmail("contact@business.gov");              // Government domain
TestEmail("sales@store.edu");                   // Education domain

// Different TLD variations
Console.WriteLine("\n  Various TLD Formats:");
TestEmail("user@domain.io");                    // Tech TLD
TestEmail("test@example.ai");                   // AI domain
TestEmail("admin@site.tech");                   // Tech domain
TestEmail("info@company.dev");                  // Developer domain
TestEmail("contact@service.app");               // App domain
TestEmail("user@example.museum");               // Long TLD
TestEmail("test@domain.travel");                // Travel TLD
TestEmail("admin@site.international");          // Very long TLD

// Subdomain variations
Console.WriteLine("\n  Subdomain Variations:");
TestEmail("user@mail.example.com");             // Single subdomain
TestEmail("test@support.help.company.org");     // Multiple subdomains
TestEmail("admin@www.site.co.za");              // www subdomain
TestEmail("api@v1.service.dev");                // API subdomain
TestEmail("user@staging.app.example.com");      // Deep subdomain

// Local part variations
Console.WriteLine("\n  Local Part Variations:");
TestEmail("a@example.com");                     // Single character
TestEmail("ab@example.com");                    // Two characters
TestEmail("user123@example.com");               // With numbers
TestEmail("user.name@example.com");             // With dot
TestEmail("user_name@example.com");             // With underscore
TestEmail("user-name@example.com");             // With hyphen
TestEmail("first.middle.last@example.com");     // Multiple dots
TestEmail("user+tag@example.com");              // Plus addressing
TestEmail("user+multiple+tags@example.com");    // Multiple plus tags
TestEmail("test.email.with+symbol@example.com"); // Mixed separators

// Special characters in local part
Console.WriteLine("\n  Special Characters in Local Part:");
TestEmail("user!@example.com");                 // Exclamation
TestEmail("user#@example.com");                 // Hash
TestEmail("user$@example.com");                 // Dollar sign
TestEmail("user%@example.com");                 // Percent
TestEmail("user&@example.com");                 // Ampersand
TestEmail("user'@example.com");                 // Apostrophe
TestEmail("user*@example.com");                 // Asterisk
TestEmail("user=@example.com");                 // Equals
TestEmail("user?@example.com");                 // Question mark
TestEmail("user^@example.com");                 // Caret
TestEmail("user`@example.com");                 // Backtick
TestEmail("user{@example.com");                 // Left brace
TestEmail("user|@example.com");                 // Pipe
TestEmail("user}@example.com");                 // Right brace
TestEmail("user~@example.com");                 // Tilde

// Long local parts and domains
Console.WriteLine("\n  Length Variations:");
TestEmail("a@b.co");                           // Minimal valid email
TestEmail("verylongusernamethatistestingthelimits@example.com"); // Long local part
TestEmail("user@verylongdomainnameforthislengthtest.com"); // Long domain
TestEmail("test@sub.very.long.subdomain.chain.example.org"); // Long subdomain chain

// Numeric variations
Console.WriteLine("\n  Numeric Variations:");
TestEmail("123@example.com");                   // All numeric local
TestEmail("user@123example.com");               // Numeric start domain
TestEmail("user@example123.com");               // Numeric in domain
TestEmail("123user@123example.com");            // Mixed numeric
TestEmail("user@sub123.example456.com");        // Numeric in subdomains

// Case sensitivity tests
Console.WriteLine("\n  Case Variations:");
TestEmail("USER@EXAMPLE.COM");                  // All uppercase
TestEmail("User@Example.Com");                  // Mixed case
TestEmail("uSeR@eXaMpLe.CoM");                  // Random case
TestEmail("test@SUBDOMAIN.EXAMPLE.COM");        // Uppercase domain

// Country code domains
Console.WriteLine("\n  Country Code Domains:");
TestEmail("user@example.us");                   // United States
TestEmail("user@example.uk");                   // United Kingdom
TestEmail("user@example.de");                   // Germany
TestEmail("user@example.fr");                   // France
TestEmail("user@example.jp");                   // Japan
TestEmail("user@example.au");                   // Australia
TestEmail("user@example.ca");                   // Canada
TestEmail("user@example.br");                   // Brazil
TestEmail("user@example.in");                   // India
TestEmail("user@example.cn");                   // China
TestEmail("user@example.ru");                   // Russia

// Edge cases that should be valid
Console.WriteLine("\n  Edge Cases (Should be Valid):");
TestEmail("x@y.co");                           // Minimal
TestEmail("test@localhost.localdomain");        // Localhost style
TestEmail("postmaster@example.com");            // Common system account
TestEmail("admin@example.com");                 // Common admin account
TestEmail("noreply@example.com");               // Common no-reply account
TestEmail("support@example.com");               // Common support account

// Comprehensive invalid email test cases
Console.WriteLine("\n--- Comprehensive Invalid Examples ---");

// Missing components
Console.WriteLine("\n  Missing Components:");
TestEmail("");                                  // Empty string
TestEmail("@");                                 // Only @ symbol
TestEmail("@example.com");                      // Missing local part
TestEmail("user@");                             // Missing domain
TestEmail("user");                              // Missing @ and domain
TestEmail("example.com");                       // Missing @ and local part
TestEmail("@.com");                             // Missing local part and domain name

// Multiple @ symbols
Console.WriteLine("\n  Multiple @ Symbols:");
TestEmail("user@@example.com");                 // Double @
TestEmail("user@domain@example.com");           // @ in domain
TestEmail("us@er@example.com");                 // @ in local part
TestEmail("@@example.com");                     // Multiple @ at start
TestEmail("user@@");                            // Multiple @ at end
TestEmail("@user@example.com");                 // @ at beginning and middle

// Invalid local part - dots
Console.WriteLine("\n  Invalid Local Part - Dots:");
TestEmail(".user@example.com");                 // Dot at start
TestEmail("user.@example.com");                 // Dot at end
TestEmail("user..name@example.com");            // Consecutive dots
TestEmail("..user@example.com");                // Multiple dots at start
TestEmail("user..@example.com");                // Multiple dots at end
TestEmail("u..s..e..r@example.com");            // Multiple consecutive dots
TestEmail("...@example.com");                   // Only dots in local part

// Invalid local part - spaces and control characters
Console.WriteLine("\n  Invalid Local Part - Spaces/Control:");
TestEmail("user name@example.com");             // Space in unquoted local
TestEmail(" user@example.com");                 // Leading space
TestEmail("user @example.com");                 // Trailing space
TestEmail("u ser@example.com");                 // Middle space
TestEmail("user\t@example.com");                // Tab character
TestEmail("user\n@example.com");                // Newline character
TestEmail("user\r@example.com");                // Carriage return

// Invalid local part - other characters
Console.WriteLine("\n  Invalid Local Part - Invalid Characters:");
TestEmail("user<@example.com");                 // Less than
TestEmail("user>@example.com");                 // Greater than
TestEmail("user(@example.com");                 // Unmatched parenthesis
TestEmail("user)@example.com");                 // Unmatched parenthesis
TestEmail("user[@example.com");                 // Unmatched bracket
TestEmail("user]@example.com");                 // Unmatched bracket
TestEmail("user\\@example.com");                // Unescaped backslash
TestEmail("user,@example.com");                 // Comma
TestEmail("user;@example.com");                 // Semicolon
TestEmail("user:@example.com");                 // Colon

// Invalid quoted strings
Console.WriteLine("\n  Invalid Quoted Strings:");
TestEmail("\"user@example.com");                // Missing closing quote
TestEmail("user\"@example.com");                // Missing opening quote
TestEmail("\"user\"name\"@example.com");        // Multiple quotes
TestEmail("\"user\n\"@example.com");            // Newline in quotes
TestEmail("\"user\r\"@example.com");            // Carriage return in quotes
TestEmail("\"\"@example.com");                  // Empty quoted string
TestEmail("\"user\\\"@example.com");            // Incomplete escape
TestEmail("\"user\0\"@example.com");            // Null character

// Domain validation - missing parts
Console.WriteLine("\n  Domain - Missing Parts:");
TestEmail("user@");                             // No domain
TestEmail("user@.");                            // Only dot
TestEmail("user@..");                           // Only dots
TestEmail("user@..com");                        // Leading dots
TestEmail("user@com.");                         // Trailing dot
TestEmail("user@.com.");                        // Leading and trailing dots

// Domain validation - invalid structure
Console.WriteLine("\n  Domain - Invalid Structure:");
TestEmail("user@domain");                       // Missing TLD
TestEmail("user@.domain.com");                  // Leading dot
TestEmail("user@domain.com.");                  // Trailing dot
TestEmail("user@domain..com");                  // Consecutive dots
TestEmail("user@domain...com");                 // Multiple consecutive dots
TestEmail("user@.domain..com.");                // Multiple dot issues
TestEmail("user@domain.c");                     // TLD too short (debatable)
TestEmail("user@-domain.com");                  // Domain starts with hyphen
TestEmail("user@domain-.com");                  // Domain ends with hyphen

// Domain validation - invalid characters
Console.WriteLine("\n  Domain - Invalid Characters:");
TestEmail("user@domain$.com");                  // Dollar sign
TestEmail("user@domain#.com");                  // Hash
TestEmail("user@domain@.com");                  // @ in domain
TestEmail("user@domain!.com");                  // Exclamation
TestEmail("user@domain%.com");                  // Percent
TestEmail("user@domain&.com");                  // Ampersand
TestEmail("user@domain*.com");                  // Asterisk
TestEmail("user@domain+.com");                  // Plus
TestEmail("user@domain=.com");                  // Equals
TestEmail("user@domain?.com");                  // Question mark
TestEmail("user@domain^.com");                  // Caret
TestEmail("user@domain_.com");                  // Underscore in domain
TestEmail("user@domain|.com");                  // Pipe
TestEmail("user@domain~.com");                  // Tilde
TestEmail("user@domain/.com");                  // Forward slash
TestEmail("user@domain\\.com");                 // Backslash

// Domain validation - spaces
Console.WriteLine("\n  Domain - Spaces:");
TestEmail("user@domain .com");                  // Space in domain
TestEmail("user@ domain.com");                  // Space before domain
TestEmail("user@domain.com ");                  // Space after domain
TestEmail("user@do main.com");                  // Space in middle
TestEmail("user@domain. com");                  // Space before TLD
TestEmail("user@domain .co m");                 // Multiple spaces

// IP literal validation
Console.WriteLine("\n  IP Literal Issues:");
TestEmail("user@[");                           // Incomplete bracket
TestEmail("user@]");                           // Wrong bracket
TestEmail("user@[]");                          // Empty brackets
TestEmail("user@[192.168.1]");                 // Incomplete IPv4
TestEmail("user@[192.168.1.1.1]");             // Too many IPv4 parts
TestEmail("user@[256.1.1.1]");                 // IPv4 out of range
TestEmail("user@[192.168.01.1]");              // IPv4 leading zero
TestEmail("user@[192.168.1.-1]");              // IPv4 negative
TestEmail("user@[IPv6]");                      // Incomplete IPv6
TestEmail("user@[IPv6:]");                     // Empty IPv6
TestEmail("user@[IPv6:invalid]");              // Invalid IPv6
TestEmail("user@[192.168.1.1");               // Missing closing bracket
TestEmail("user@192.168.1.1]");               // Missing opening bracket

// Length violations
Console.WriteLine("\n  Length Violations:");
TestEmail("a" + new string('b', 63) + "@example.com");           // Local part too long (>64)
TestEmail("user@" + new string('a', 250) + ".com");             // Domain too long (>253)
TestEmail(new string('a', 300) + "@example.com");               // Total too long (>320)
TestEmail("user@" + string.Join(".", new string[70].Select((_, i) => "a").ToArray()) + ".com"); // Too many labels

// Mixed invalid cases
Console.WriteLine("\n  Mixed Invalid Cases:");
TestEmail("user@domain..com.");                 // Multiple domain issues
TestEmail(".user.@domain..com");                // Multiple local and domain issues
TestEmail("\"user@domain.com");                 // Quoted local missing close + domain issues
TestEmail("user name@domain .com");             // Space in both parts
TestEmail("user@@domain..com");                 // Multiple @ and domain dots
TestEmail("@user@.domain.com");                 // Multiple @ and domain dot issues
TestEmail("user\\name@domain$.com");            // Unescaped backslash and domain $
TestEmail("\"user\n\"@domain .com");            // Newline in quotes and space in domain

// Unicode and encoding edge cases
Console.WriteLine("\n  Unicode/Encoding Edge Cases:");
TestEmail("user\x00@example.com");              // Null character
TestEmail("user\x7f@example.com");              // DEL character  
TestEmail("user@\x00example.com");              // Null in domain
TestEmail("user@example\x7f.com");              // DEL in domain

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