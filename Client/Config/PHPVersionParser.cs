//-----------------------------------------------------------------------
// <copyright>
// Copyright (C) Ruslan Yakushev for the PHP Manager for IIS project.
//
// This file is subject to the terms and conditions of the Microsoft Public License (MS-PL).
// See http://www.microsoft.com/opensource/licenses.mspx#Ms-PL for more details.
// </copyright>
//----------------------------------------------------------------------- 

using System;
using System.Globalization;
using System.Text.RegularExpressions;

namespace Web.Management.PHP.Config
{

    // Shared by the client and the server: this file is linked into both projects.
    internal static class PHPVersionParser
    {
        // Matches the leading numeric part of a PHP version: major.minor[.build[.revision]].
        // Anything after it (for example "-pl3" in "7.4.33-pl3") is ignored.
        private static readonly Regex VersionRegex = new Regex(
            @"^\s*v?(?<major>\d+)\.(?<minor>\d+)(?:\.(?<build>\d+))?(?:\.(?<revision>\d+))?(?!\d)",
            RegexOptions.CultureInvariant);

        /// <summary>
        /// Converts a PHP version string, such as "8.1.2" or "7.4.33-pl3", to a <see cref="Version"/>.
        /// Unlike <c>new Version(string)</c> it tolerates a trailing suffix.
        /// </summary>
        /// <returns>
        /// The parsed version with two to four components (as many as the string has),
        /// or null if the string does not start with major.minor.
        /// </returns>
        public static Version ParsePhpVersion(string version)
        {
            if (String.IsNullOrEmpty(version))
            {
                return null;
            }

            var match = VersionRegex.Match(version);
            if (!match.Success)
            {
                return null;
            }

            int major;
            int minor;
            if (!Int32.TryParse(match.Groups["major"].Value, NumberStyles.None, CultureInfo.InvariantCulture, out major) ||
                !Int32.TryParse(match.Groups["minor"].Value, NumberStyles.None, CultureInfo.InvariantCulture, out minor))
            {
                return null;
            }

            if (!match.Groups["build"].Success)
            {
                return new Version(major, minor);
            }

            int build;
            if (!Int32.TryParse(match.Groups["build"].Value, NumberStyles.None, CultureInfo.InvariantCulture, out build))
            {
                return null;
            }

            if (!match.Groups["revision"].Success)
            {
                return new Version(major, minor, build);
            }

            int revision;
            if (!Int32.TryParse(match.Groups["revision"].Value, NumberStyles.None, CultureInfo.InvariantCulture, out revision))
            {
                return null;
            }

            return new Version(major, minor, build, revision);
        }
    }

}