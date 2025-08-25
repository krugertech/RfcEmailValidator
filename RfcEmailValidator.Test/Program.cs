using KrugerTech.Net;

int totalTests = 0;
int passedTests = 0;
int failedTests = 0;

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
TestEmail("δοκιμή@παράδειγμα.δοκιμή");          // Valid (Greek characters) - MailAddress support varies
TestEmail("我買@屋企.香港");                     // Valid (Chinese characters) - MailAddress support varies


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
TestEmail("invalid-email", false);                     // Invalid (missing @ and domain)
TestEmail("user@.com", false);                         // Invalid (domain starts with dot)
TestEmail("user@domain", false);                       // Invalid (missing TLD)
TestEmail("user@domain..com", false);                  // Invalid (consecutive dots in domain)
TestEmail("user@domain.com.", false);                  // Invalid (domain ends with dot)
TestEmail(".user@domain.com", false);                  // Invalid (local part starts with dot)
TestEmail("user..name@domain.com", false);             // Invalid (consecutive dots in unquoted local part)
TestEmail("user name@domain.com", false);              // Invalid (space in unquoted local part)

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
TestEmail("", false);                                  // Empty string
TestEmail("@", false);                                 // Only @ symbol
TestEmail("@example.com", false);                      // Missing local part
TestEmail("user@", false);                             // Missing domain
TestEmail("user", false);                              // Missing @ and domain
TestEmail("example.com", false);                       // Missing @ and local part
TestEmail("@.com", false);                             // Missing local part and domain name

// Multiple @ symbols
Console.WriteLine("\n  Multiple @ Symbols:");
TestEmail("user@@example.com", false);                 // Double @
TestEmail("user@domain@example.com", false);           // @ in domain
TestEmail("us@er@example.com", false);                 // @ in local part
TestEmail("@@example.com", false);                     // Multiple @ at start
TestEmail("user@@", false);                            // Multiple @ at end
TestEmail("@user@example.com", false);                 // @ at beginning and middle

// Invalid local part - dots
Console.WriteLine("\n  Invalid Local Part - Dots:");
TestEmail(".user@example.com", false);                 // Dot at start
TestEmail("user.@example.com", false);                 // Dot at end
TestEmail("user..name@example.com", false);            // Consecutive dots
TestEmail("..user@example.com", false);                // Multiple dots at start
TestEmail("user..@example.com", false);                // Multiple dots at end
TestEmail("u..s..e..r@example.com", false);            // Multiple consecutive dots
TestEmail("...@example.com", false);                   // Only dots in local part

// Invalid local part - spaces and control characters
Console.WriteLine("\n  Invalid Local Part - Spaces/Control:");
TestEmail("user name@example.com", false);             // Space in unquoted local
TestEmail(" user@example.com", false);                 // Leading space
TestEmail("user @example.com", false);                 // Trailing space
TestEmail("u ser@example.com", false);                 // Middle space
TestEmail("user\t@example.com", false);                // Tab character
TestEmail("user\n@example.com", false);                // Newline character
TestEmail("user\r@example.com", false);                // Carriage return

// Invalid local part - other characters
Console.WriteLine("\n  Invalid Local Part - Invalid Characters:");
TestEmail("user<@example.com", false);                 // Less than
TestEmail("user>@example.com", false);                 // Greater than
TestEmail("user(@example.com", false);                 // Unmatched parenthesis
TestEmail("user)@example.com", false);                 // Unmatched parenthesis
TestEmail("user[@example.com", false);                 // Unmatched bracket
TestEmail("user]@example.com", false);                 // Unmatched bracket
TestEmail("user\\@example.com", false);                // Unescaped backslash
TestEmail("user,@example.com", false);                 // Comma
TestEmail("user;@example.com", false);                 // Semicolon
TestEmail("user:@example.com", false);                 // Colon

// Invalid quoted strings
Console.WriteLine("\n  Invalid Quoted Strings:");
TestEmail("\"user@example.com", false);                // Missing closing quote
TestEmail("user\"@example.com", false);                // Missing opening quote
TestEmail("\"user\"name\"@example.com", false);        // Multiple quotes
TestEmail("\"user\n\"@example.com", false);            // Newline in quotes
TestEmail("\"user\r\"@example.com", false);            // Carriage return in quotes
TestEmail("\"\"@example.com", false);                  // Empty quoted string
TestEmail("\"user\\\"@example.com", false);            // Incomplete escape
TestEmail("\"user\0\"@example.com", false);            // Null character

// Domain validation - missing parts
Console.WriteLine("\n  Domain - Missing Parts:");
TestEmail("user@", false);                             // No domain
TestEmail("user@.", false);                            // Only dot
TestEmail("user@..", false);                           // Only dots
TestEmail("user@..com", false);                        // Leading dots
TestEmail("user@com.", false);                         // Trailing dot
TestEmail("user@.com.", false);                        // Leading and trailing dots

// Domain validation - invalid structure
Console.WriteLine("\n  Domain - Invalid Structure:");
TestEmail("user@domain", false);                       // Missing TLD
TestEmail("user@.domain.com", false);                  // Leading dot
TestEmail("user@domain.com.", false);                  // Trailing dot
TestEmail("user@domain..com", false);                  // Consecutive dots
TestEmail("user@domain...com", false);                 // Multiple consecutive dots
TestEmail("user@.domain..com.", false);                // Multiple dot issues
TestEmail("user@domain.c", false);                     // TLD too short (debatable)
TestEmail("user@-domain.com", false);                  // Domain starts with hyphen
TestEmail("user@domain-.com", false);                  // Domain ends with hyphen

// Domain validation - invalid characters
Console.WriteLine("\n  Domain - Invalid Characters:");
TestEmail("user@domain$.com", false);                  // Dollar sign
TestEmail("user@domain#.com", false);                  // Hash
TestEmail("user@domain@.com", false);                  // @ in domain
TestEmail("user@domain!.com", false);                  // Exclamation
TestEmail("user@domain%.com", false);                  // Percent
TestEmail("user@domain&.com", false);                  // Ampersand
TestEmail("user@domain*.com", false);                  // Asterisk
TestEmail("user@domain+.com", false);                  // Plus
TestEmail("user@domain=.com", false);                  // Equals
TestEmail("user@domain?.com", false);                  // Question mark
TestEmail("user@domain^.com", false);                  // Caret
TestEmail("user@domain_.com", false);                  // Underscore in domain
TestEmail("user@domain|.com", false);                  // Pipe
TestEmail("user@domain~.com", false);                  // Tilde
TestEmail("user@domain/.com", false);                  // Forward slash
TestEmail("user@domain\\.com", false);                 // Backslash

// Domain validation - spaces
Console.WriteLine("\n  Domain - Spaces:");
TestEmail("user@domain .com", false);                  // Space in domain
TestEmail("user@ domain.com", false);                  // Space before domain
TestEmail("user@domain.com ", false);                  // Space after domain
TestEmail("user@do main.com", false);                  // Space in middle
TestEmail("user@domain. com", false);                  // Space before TLD
TestEmail("user@domain .co m", false);                 // Multiple spaces

// IP literal validation
Console.WriteLine("\n  IP Literal Issues:");
TestEmail("user@[", false);                           // Incomplete bracket
TestEmail("user@]", false);                           // Wrong bracket
TestEmail("user@[]", false);                          // Empty brackets
TestEmail("user@[192.168.1]", false);                 // Incomplete IPv4
TestEmail("user@[192.168.1.1.1]", false);             // Too many IPv4 parts
TestEmail("user@[256.1.1.1]", false);                 // IPv4 out of range
TestEmail("user@[192.168.01.1]", false);              // IPv4 leading zero
TestEmail("user@[192.168.1.-1]", false);              // IPv4 negative
TestEmail("user@[IPv6]", false);                      // Incomplete IPv6
TestEmail("user@[IPv6:]", false);                     // Empty IPv6
TestEmail("user@[IPv6:invalid]", false);              // Invalid IPv6
TestEmail("user@[192.168.1.1", false);               // Missing closing bracket
TestEmail("user@192.168.1.1]", false);               // Missing opening bracket

// Length violations
Console.WriteLine("\n  Length Violations:");
TestEmail("a" + new string('b', 63) + "@example.com", false);           // Local part too long (>64)
TestEmail("user@" + new string('a', 250) + ".com", false);             // Domain too long (>253)
TestEmail(new string('a', 300) + "@example.com", false);               // Total too long (>320)
TestEmail("user@" + string.Join(".", Enumerable.Repeat("a", 130)) + ".com", false); // Too many labels

// Mixed invalid cases
Console.WriteLine("\n  Mixed Invalid Cases:");
TestEmail("user@domain..com.", false);                 // Multiple domain issues
TestEmail(".user.@domain..com", false);                // Multiple local and domain issues
TestEmail("\"user@domain.com", false);                 // Quoted local missing close + domain issues
TestEmail("user name@domain .com", false);             // Space in both parts
TestEmail("user@@domain..com", false);                 // Multiple @ and domain dots
TestEmail("@user@.domain.com", false);                 // Multiple @ and domain dot issues
TestEmail("user\\name@domain$.com", false);            // Unescaped backslash and domain $
TestEmail("\"user\n\"@domain .com", false);            // Newline in quotes and space in domain

// Unicode and encoding edge cases
Console.WriteLine("\n  Unicode/Encoding Edge Cases:");
TestEmail("user\x00@example.com", false);              // Null character
TestEmail("user\x7f@example.com", false);              // DEL character
TestEmail("user@\x00example.com", false);              // Null in domain
TestEmail("user@example\x7f.com", false);              // DEL in domain

// Whitespace edge cases
Console.WriteLine("\n  Whitespace Edge Cases:");
TestEmail(" user@example.com", false);                 // Leading space
TestEmail("user@example.com ", false);                 // Trailing space
TestEmail(" user@example.com ", false);                // Leading and trailing space
TestEmail("\tuser@example.com", false);                // Leading tab
TestEmail("user@example.com\t", false);                // Trailing tab

// TLD length issues
Console.WriteLine("\n  TLD Length Issues:");
TestEmail("user@domain.c", false);                     // Single character TLD
TestEmail("user@domain.", false);                      // Empty TLD
TestEmail("user@domain.a", false);                     // Single letter TLD

// Excessive subdomain labels
Console.WriteLine("\n  Excessive Domain Labels:");
TestEmail("user@" + string.Join(".", Enumerable.Repeat("a", 130)) + ".com", false); // Too many labels

// Display final test results
Console.WriteLine($"\n=== TEST RESULTS SUMMARY ===");
if (failedTests == 0)
{
    Console.ForegroundColor = ConsoleColor.Green;
    Console.WriteLine($"✅ ALL TESTS PASSED! ({passedTests}/{totalTests})");
    Console.ResetColor();
}
else
{
    Console.ForegroundColor = ConsoleColor.Red;
    Console.WriteLine($"❌ {failedTests} TESTS FAILED out of {totalTests} total tests");
    Console.ResetColor();
    Console.ForegroundColor = ConsoleColor.Green;
    Console.WriteLine($"✅ {passedTests} tests passed");
    Console.ResetColor();
}

void TestEmail(string email, bool expectedValid = true)
{
    totalTests++;
    bool isValid = EmailValidator.IsRfcCompliant(email);
    bool testPassed = (isValid == expectedValid);

    if (testPassed)
    {
        passedTests++;
        Console.WriteLine($"'{email,-40}' is {(isValid ? "VALID" : "INVALID")}");
    }
    else
    {
        failedTests++;
        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine($"'{email,-40}' is {(isValid ? "VALID" : "INVALID")} ❌ EXPECTED {(expectedValid ? "VALID" : "INVALID")}");
        Console.ResetColor();
    }
}

void TestEmailWithParsing(string email, bool expectedValid = true)
{
    totalTests++;
    bool isValid = EmailValidator.IsRfcCompliant(email);
    bool testPassed = (isValid == expectedValid);
    var parsed = EmailValidator.ParseEmail(email);

    if (testPassed)
    {
        passedTests++;
        Console.WriteLine($"'{email,-40}' is {(isValid ? "VALID" : "INVALID")}");
    }
    else
    {
        failedTests++;
        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine($"'{email,-40}' is {(isValid ? "VALID" : "INVALID")} ❌ EXPECTED {(expectedValid ? "VALID" : "INVALID")}");
        Console.ResetColor();
    }

    if (parsed != null && isValid)
    {
        Console.WriteLine($"  Display Name: '{parsed.DisplayName}'");
        Console.WriteLine($"  Local Part: '{parsed.LocalPart}'");
        Console.WriteLine($"  Domain: '{parsed.Domain}'");
        Console.WriteLine($"  Full Address: '{parsed.Address}'");
    }
    Console.WriteLine();
}