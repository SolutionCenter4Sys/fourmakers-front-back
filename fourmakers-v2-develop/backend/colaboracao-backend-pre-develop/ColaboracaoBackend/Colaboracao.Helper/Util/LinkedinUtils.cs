using System;
using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace Colaboracao.Helper.Util;

public static class LinkedinUtils
{
    public static string GetVanityName(string linkedinUrl)
    {
        try
        {
            if(linkedinUrl.Contains("linkedin.com"))
            {
                string pattern = @"(?<=/in/)[^/]+";
                var match = Regex.Match(linkedinUrl, pattern);

                if (!match.Success)
                    throw new ValidationException("Url inválida");

                return match.Value;
            }
            else
            {
                return linkedinUrl;
            }
                
        }
        catch (Exception)
        {
            throw new ValidationException("Url inválida");
        }
    }
}