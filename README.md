# RFC Compliant Email Address Validator

A lightweight email address validator that implements full support for RFC5322 and partial support for RFC6531 & RFC2822 without dependencies on System.Net.Mail.

## NuGet Installation

```sh
dotnet add package Krugertech.Net.RfcEmailValidator
```

## Usage

### Basic Validation
```csharp
using KrugerTech.Net;

bool isValid = EmailValidator.IsRfcCompliant("user@example.com");
```

### Email Parsing
```csharp
using KrugerTech.Net;

EmailAddress? parsed = EmailValidator.ParseEmail("\"John Doe\" <john.doe@example.com>");
if (parsed != null)
{
    Console.WriteLine($"Display Name: {parsed.DisplayName}");
    Console.WriteLine($"Local Part: {parsed.LocalPart}");
    Console.WriteLine($"Domain: {parsed.Domain}");
    Console.WriteLine($"Full Address: {parsed.Address}");
}
```

### Typical Usage Scenarios

- **Validate user input:**
    ```csharp
    if (EmailValidator.IsRfcCompliant(userInput)) {
            // Accept email
    } else {
            // Show validation error
    }
    ```

- **Extract components for storage or display:**
    ```csharp
    var email = EmailValidator.ParseEmail(input);
    if (email != null) {
            SaveToDatabase(email.LocalPart, email.Domain);
    }
    ```

## Current RFC Coverage

### ✅ RFC 5322 (Basic Email Format) - **Fully Supported**
- ✅ Basic local-part@domain structure
- ✅ Dot-atom format in local parts (user.name@domain.com)
- ✅ Quoted strings in local parts ("john smith"@domain.com)
- ✅ Domain validation with proper TLD requirements (minimum 2 characters)
- ✅ IP literals for domains [192.168.1.1]
- ✅ Length restrictions (local part <64 chars, domain <253 chars, total <320 chars)
- ✅ Special characters in local parts (!#$%&'*+-/=?^_`{|}~)
- ✅ Comments handling (strips comments in parentheses)
- ✅ Display name parsing with angle brackets
- ✅ Proper whitespace validation (rejects leading/trailing spaces)
- ✅ Domain label count limits (max 10 levels)

### ✅ RFC 2822 Support - **Partially Supported**
- ✅ Basic address parsing
- ✅ Display name parsing with angle brackets ("Name" <email@domain.com>)
- ✅ Some folding whitespace handling
- ✅ Basic comment parsing and removal

### ✅ RFC 6531 (Internationalization) - **Partially Supported**
- ✅ International characters in local parts (éléonore@example.com)
- ✅ International domain names (user@münchen.de)
- ✅ Unicode character support in domains and local parts

## Missing/Incomplete Areas

### ❌ RFC 5322 Gaps
1. **Obsolete syntax support** - RFC 5322 requires backward compatibility with some obsolete formats
2. **Complete folding whitespace (FWS) handling** - Limited support for complex whitespace folding
3. **Proper CFWS (comments/folding whitespace) parsing** - Basic comment removal, not full CFWS parsing
4. **Advanced domain literal validation** - IPv6 validation is simplified, needs full RFC compliance
5. **Complete quoted-pair handling** - Limited escape sequence support in quoted strings
6. **Nested comment parsing** - Comments within comments not fully supported

### ❌ RFC 6531 Gaps
1. **SMTPUTF8 extension considerations** - No specific SMTP UTF-8 extension handling
2. **Complete UTF-8 validation and normalization** - No Unicode normalization (NFC/NFD)
3. **International domain name (IDN) encoding/decoding** - No Punycode conversion
4. **EAI (Email Address Internationalization) edge cases** - Some complex internationalization scenarios

### ❌ RFC 2822 Legacy Support
1. **Route addressing** - Obsolete `<@route:user@domain>` format not supported
2. **Group syntax** - `Group: user1@domain1, user2@domain2;` format not supported
3. **Complete obsolete format support** - Various legacy formats from RFC 2822
