using System;
using System.Text;
using System.Text.RegularExpressions;
using System.Globalization;

namespace QuickDevTest
{
    /// <summary>
    /// A lightweight email address validator that implements RFC 5322 compliance
    /// without dependencies on System.Net.Mail
    /// </summary>
    public static class EmailValidator
    {

        /// <summary>
        /// Validates an email address for RFC 5322 compliance
        /// </summary>
        /// <param name="emailAddress">The email address to validate</param>
        /// <returns>True if the email address is valid, false otherwise</returns>
        public static bool IsValidEmail(string emailAddress)
        {
            if (string.IsNullOrWhiteSpace(emailAddress))
                return false;

            // Check for leading/trailing whitespace (should be invalid)
            if (emailAddress != emailAddress.Trim())
                return false;

            try
            {
                // Remove any comments first (text between parentheses)
                string cleanedEmail = RemoveComments(emailAddress.Trim());
                
                // Basic length check
                if (cleanedEmail.Length > 320) // RFC 5321 limit
                    return false;

                // Handle display name format: "Display Name" <email@domain.com> or Display Name <email@domain.com>
                string actualEmail = cleanedEmail;
                if (cleanedEmail.Contains('<') && cleanedEmail.EndsWith('>'))
                {
                    int angleStart = cleanedEmail.LastIndexOf('<');
                    int angleEnd = cleanedEmail.LastIndexOf('>');
                    
                    if (angleStart >= 0 && angleEnd > angleStart)
                    {
                        actualEmail = cleanedEmail.Substring(angleStart + 1, angleEnd - angleStart - 1).Trim();
                        
                        // Validate the display name part if present
                        if (angleStart > 0)
                        {
                            string displayPart = cleanedEmail.Substring(0, angleStart).Trim();
                            if (!ValidateDisplayName(displayPart))
                                return false;
                        }
                    }
                    else
                    {
                        return false; // Malformed angle brackets
                    }
                }

                // Split into local and domain parts
                int atIndex = actualEmail.LastIndexOf('@');
                if (atIndex <= 0 || atIndex == actualEmail.Length - 1)
                    return false;

                string localPart = actualEmail.Substring(0, atIndex);
                string domainPart = actualEmail.Substring(atIndex + 1);

                // Additional validation
                return ValidateLocalPart(localPart) && ValidateDomainPart(domainPart);
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Parses an email address into its components
        /// </summary>
        /// <param name="emailAddress">The email address to parse</param>
        /// <returns>EmailAddress object with parsed components, or null if invalid</returns>
        public static EmailAddress? ParseEmail(string emailAddress)
        {
            if (!IsValidEmail(emailAddress))
                return null;

            try
            {
                string originalEmail = emailAddress.Trim();
                string displayName = "";
                string actualEmail = originalEmail;

                // Check if there's a display name (format: "Display Name" <email@domain.com>)
                if (originalEmail.Contains('<') && originalEmail.EndsWith('>'))
                {
                    int angleStart = originalEmail.IndexOf('<');
                    int angleEnd = originalEmail.LastIndexOf('>');
                    
                    if (angleStart > 0 && angleEnd > angleStart)
                    {
                        displayName = originalEmail.Substring(0, angleStart).Trim();
                        actualEmail = originalEmail.Substring(angleStart + 1, angleEnd - angleStart - 1);
                        
                        // Remove quotes from display name
                        if (displayName.StartsWith('"') && displayName.EndsWith('"'))
                        {
                            displayName = displayName.Substring(1, displayName.Length - 2);
                        }
                    }
                }

                // Remove comments from the email part
                actualEmail = RemoveComments(actualEmail);

                int atIndex = actualEmail.LastIndexOf('@');
                string localPart = actualEmail.Substring(0, atIndex);
                string domainPart = actualEmail.Substring(atIndex + 1);

                return new EmailAddress(displayName, localPart, domainPart);
            }
            catch
            {
                return null;
            }
        }

        private static bool ValidateDisplayName(string displayName)
        {
            if (string.IsNullOrEmpty(displayName))
                return true;

            displayName = displayName.Trim();
            
            // Handle quoted display names
            if (displayName.StartsWith('"') && displayName.EndsWith('"'))
            {
                return ValidateQuotedString(displayName);
            }

            // For unquoted display names, check for valid characters
            // Allow most printable characters, but be more permissive than local part
            foreach (char c in displayName)
            {
                if (c < 32 || c > 126) // Outside printable ASCII range
                {
                    // Allow some international characters
                    if (!char.IsLetter(c) && !char.IsDigit(c) && c != ' ')
                        return false;
                }
            }

            return true;
        }

        private static bool ValidateLocalPart(string localPart)
        {
            if (string.IsNullOrEmpty(localPart) || localPart.Length > 64) // RFC 5321 limit
                return false;

            // Handle quoted strings
            if (localPart.StartsWith('"') && localPart.EndsWith('"'))
            {
                return ValidateQuotedString(localPart);
            }

            // Handle dot-atom format
            if (localPart.StartsWith('.') || localPart.EndsWith('.'))
                return false;

            if (localPart.Contains(".."))
                return false;

            // Check for valid characters in unquoted local part
            foreach (char c in localPart)
            {
                if (!IsValidLocalPartChar(c))
                    return false;
            }

            return true;
        }

        private static bool ValidateQuotedString(string quotedString)
        {
            if (quotedString.Length < 2)
                return false;

            string content = quotedString.Substring(1, quotedString.Length - 2);
            
            // Empty quoted string is invalid - must have at least one character
            if (content.Length == 0)
                return false;
            
            for (int i = 0; i < content.Length; i++)
            {
                char c = content[i];
                
                // Handle escaped characters
                if (c == '\\')
                {
                    if (i + 1 >= content.Length)
                        return false;
                    i++; // Skip the next character as it's escaped
                    continue;
                }

                // Check if character is valid in quoted string
                // Allow printable ASCII characters except unescaped quotes
                if (c == '"' || c < 32 || c == 127)
                {
                    return false;
                }
            }

            return true;
        }

        private static bool ValidateDomainPart(string domainPart)
        {
            if (string.IsNullOrEmpty(domainPart) || domainPart.Length > 253) // RFC 5321 limit
                return false;

            // Handle IP literals [192.168.1.1] or [IPv6:2001:db8::1]
            if (domainPart.StartsWith('[') && domainPart.EndsWith(']'))
            {
                return ValidateIpLiteral(domainPart);
            }

            // Handle regular domain names
            if (domainPart.StartsWith('.') || domainPart.EndsWith('.'))
                return false;

            if (domainPart.Contains(".."))
                return false;

            string[] labels = domainPart.Split('.');
            if (labels.Length < 2) // Must have at least domain.tld (except for IP literals)
                return false;
            
            if (labels.Length > 127) // Reasonable limit on number of labels to prevent abuse
                return false;

            // Validate TLD (last label) - must be at least 2 characters
            string tld = labels[labels.Length - 1];
            if (tld.Length < 2)
                return false;

            foreach (string label in labels)
            {
                if (!ValidateDomainLabel(label))
                    return false;
            }

            return true;
        }

        private static bool ValidateDomainLabel(string label)
        {
            if (string.IsNullOrEmpty(label) || label.Length > 63) // RFC 5321 limit
                return false;

            if (label.StartsWith('-') || label.EndsWith('-'))
                return false;

            foreach (char c in label)
            {
                // Allow letters (including international), digits, and hyphens
                if (!char.IsLetterOrDigit(c) && c != '-')
                    return false;
            }

            return true;
        }

        private static bool ValidateIpLiteral(string ipLiteral)
        {
            string content = ipLiteral.Substring(1, ipLiteral.Length - 2);
            
            // IPv6 format
            if (content.StartsWith("IPv6:", StringComparison.OrdinalIgnoreCase))
            {
                string ipv6Address = content.Substring(5);
                return IsValidIPv6(ipv6Address);
            }
            
            // IPv4 format
            return IsValidIPv4(content);
        }

        private static bool IsValidIPv4(string ip)
        {
            string[] parts = ip.Split('.');
            if (parts.Length != 4)
                return false;

            foreach (string part in parts)
            {
                if (!int.TryParse(part, out int value) || value < 0 || value > 255)
                    return false;
                
                // No leading zeros allowed except for "0"
                if (part.Length > 1 && part.StartsWith('0'))
                    return false;
            }

            return true;
        }

        private static bool IsValidIPv6(string ip)
        {
            // Simplified IPv6 validation - for production use, consider more comprehensive validation
            if (string.IsNullOrEmpty(ip))
                return false;

            // Remove any leading/trailing whitespace
            ip = ip.Trim();

            // Basic IPv6 pattern check - allow compressed format with ::
            // This is a simplified check that handles common cases
            if (ip == "::" || ip == "::1")
                return true;

            // Check for valid IPv6 characters
            foreach (char c in ip)
            {
                if (!char.IsDigit(c) && !"abcdefABCDEF:".Contains(c))
                    return false;
            }

            // Basic structure validation
            if (ip.Contains(":::")) // Too many consecutive colons
                return false;

            string[] parts = ip.Split(':');
            if (parts.Length > 8) // IPv6 has max 8 groups
                return false;

            // If we have :: compression, we should have fewer than 8 parts
            bool hasCompression = ip.Contains("::");
            if (hasCompression)
            {
                // With compression, we can have fewer parts
                return parts.Length <= 8;
            }
            else
            {
                // Without compression, we need exactly 8 parts
                if (parts.Length != 8)
                    return false;
            }

            // Validate each hexadecimal group
            foreach (string part in parts)
            {
                if (string.IsNullOrEmpty(part) && !hasCompression)
                    return false;
                
                if (!string.IsNullOrEmpty(part))
                {
                    if (part.Length > 4) // Each group is max 4 hex digits
                        return false;
                    
                    if (!Regex.IsMatch(part, "^[0-9a-fA-F]+$"))
                        return false;
                }
            }

            return true;
        }

        private static bool IsValidLocalPartChar(char c)
        {
            // Valid characters for unquoted local part
            // Allow letters (including international), digits, and specific special characters
            return char.IsLetterOrDigit(c) || 
                   "!#$%&'*+-/=?^_`{|}~.".Contains(c);
        }

        private static string RemoveComments(string input)
        {
            if (string.IsNullOrEmpty(input))
                return input;

            StringBuilder result = new StringBuilder();
            int depth = 0;
            bool inQuotes = false;
            bool escaped = false;

            foreach (char c in input)
            {
                if (escaped)
                {
                    if (depth == 0)
                        result.Append(c);
                    escaped = false;
                    continue;
                }

                if (c == '\\')
                {
                    escaped = true;
                    if (depth == 0)
                        result.Append(c);
                    continue;
                }

                if (!inQuotes && c == '(')
                {
                    depth++;
                }
                else if (!inQuotes && c == ')')
                {
                    depth--;
                }
                else if (c == '"' && depth == 0)
                {
                    inQuotes = !inQuotes;
                    result.Append(c);
                }
                else if (depth == 0)
                {
                    result.Append(c);
                }
            }

            return result.ToString();
        }
    }

    /// <summary>
    /// Represents a parsed email address with its components
    /// </summary>
    public class EmailAddress
    {
        public string DisplayName { get; }
        public string LocalPart { get; }
        public string Domain { get; }
        public string Address => $"{LocalPart}@{Domain}";

        internal EmailAddress(string displayName, string localPart, string domain)
        {
            DisplayName = displayName ?? "";
            LocalPart = localPart ?? throw new ArgumentNullException(nameof(localPart));
            Domain = domain ?? throw new ArgumentNullException(nameof(domain));
        }

        public override string ToString()
        {
            if (string.IsNullOrEmpty(DisplayName))
            {
                return Address;
            }
            else
            {
                return $"\"{DisplayName.Replace("\"", "\\\"")}\" <{Address}>";
            }
        }

        public override bool Equals(object? obj)
        {
            if (obj is EmailAddress other)
            {
                return string.Equals(Address, other.Address, StringComparison.OrdinalIgnoreCase);
            }
            return false;
        }

        public override int GetHashCode()
        {
            return StringComparer.OrdinalIgnoreCase.GetHashCode(Address);
        }
    }
}
