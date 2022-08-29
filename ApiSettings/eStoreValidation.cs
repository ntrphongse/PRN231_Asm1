using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eStoreLibrary
{
    public static class eStoreValidation
    {
        #region Validation
        /// <summary>
        /// Validate a string
        /// </summary>
        /// <param name="stringToValidate">String to Validate</param>
        /// <param name="allowEmpty">True if the string is allowed to be empty. Default is False</param>
        /// <param name="emptyErrorMessage">Error message to be thrown when the string is empty (only if allowEmpty is True)</param>
        /// <param name="minLength">Minimum length of the string. Default is -1, which means that the string is not required to be validated in the minimum length</param>
        /// <param name="minLengthErrorMessage">Error message to be thrown when the string does not meet the minimum length (only if minLength > 0)</param>
        /// <param name="maxLength">Maximum length of the string, Default is -1, which means that the string is not required to be validated in the maximum length</param>
        /// <param name="maxLengthErrorMessage">Error message to be thrown when the string does not meet the maximum length (only if maxLength > 0)</param>
        /// <returns>True if the string meets all the validation, otherwise, throw Exception with message of the corresponding message</returns>
        public static bool StringValidate(this string stringToValidate,
            bool allowEmpty = false, string emptyErrorMessage = "",
            int minLength = -1, string minLengthErrorMessage = "",
            int maxLength = -1, string maxLengthErrorMessage = "")
        {
            if (!allowEmpty)
            {
                // allowEmpty = false
                if (string.IsNullOrEmpty(stringToValidate))
                {
                    throw new ApplicationException(emptyErrorMessage);
                }
                if (minLength > 0)
                {
                    if (stringToValidate.Length < minLength)
                    {
                        throw new ApplicationException(minLengthErrorMessage);
                    }
                }
                if (maxLength > 0)
                {
                    if (stringToValidate.Length > maxLength)
                    {
                        throw new ApplicationException(maxLengthErrorMessage);
                    }
                }
            }
            else
            {
                // allowEmpty = true
                if (minLength > 0)
                {
                    if (stringToValidate == null)
                    {
                        throw new ArgumentNullException("Chuỗi không hợp lệ!!!");
                    }
                    if (stringToValidate.Length < minLength)
                    {
                        throw new ApplicationException(minLengthErrorMessage);
                    }
                }
                if (maxLength > 0)
                {
                    if (stringToValidate == null)
                    {
                        throw new ArgumentNullException("Chuỗi không hợp lệ!!!");
                    }
                    if (stringToValidate.Length > maxLength)
                    {
                        throw new ApplicationException(maxLengthErrorMessage);
                    }
                }
            }
            return true;
        }

        public static bool IsEmail(this string stringToCheck, string errorMessage)
        {
            EmailAddressAttribute emailAddressAttribute = new EmailAddressAttribute();
            if (!emailAddressAttribute.IsValid(stringToCheck))
            {
                throw new ApplicationException(errorMessage);
            }
            return true;
        }

        public static bool IntegerValidate(this int integerToValidate,
            int? minimum = null, string minErrorMessage = "",
            int? maximum = null, string maxErrorMessage = "")
        {
            if (minimum.HasValue)
            {
                if (integerToValidate < minimum.Value)
                {
                    throw new ApplicationException(minErrorMessage);
                }
            }
            if (maximum.HasValue)
            {
                if (integerToValidate > maximum.Value)
                {
                    throw new ApplicationException(maxErrorMessage);
                }
            }
            return true;
        }

        public static bool DecimalValidate(this decimal numberToValidate,
            decimal? minimum = null, string minErrorMessage = "",
            decimal? maximum = null, string maxErrorMessage = "")
        {
            if (minimum.HasValue)
            {
                if (numberToValidate < minimum.Value)
                {
                    throw new ApplicationException(minErrorMessage);
                }
            }
            if (maximum.HasValue)
            {
                if (numberToValidate > maximum.Value)
                {
                    throw new ApplicationException(maxErrorMessage);
                }
            }
            return true;
        }

        public static bool DoubleValidate(this double numberToValidate,
            double? minimum = null, string minErrorMessage = "",
            double? maximum = null, string maxErrorMessage = "")
        {
            if (minimum.HasValue)
            {
                if (numberToValidate < minimum.Value)
                {
                    throw new ApplicationException(minErrorMessage);
                }
            }
            if (maximum.HasValue)
            {
                if (numberToValidate > maximum.Value)
                {
                    throw new ApplicationException(maxErrorMessage);
                }
            }
            return true;
        }
        #endregion
    }
}
