using System;
using System.Linq;


namespace Modules.Hive
{
    // This class duplicates System.Version for lack of leading zeros (2.01 != 2.1) and
    // more than 4 version numbers (1.0.0.1.5) support
    // https://referencesource.microsoft.com/#mscorlib/system/version.cs
    public class ExtendedVersion : ICloneable, IComparable, IComparable<ExtendedVersion>, IEquatable<ExtendedVersion>
    {
        #region Fields
        
        private const char VersionSeparator = '.';
        // Always exist
        private readonly string[] splitStringRepresentation;
        private readonly int[] intRepresentation;
        // Lazy initialized
        private string stringRepresentation;
        
        #endregion

        
        
        #region Properties

        private string StringRepresentation
        {
            get
            { 
                return string.IsNullOrEmpty(stringRepresentation) ? 
                    stringRepresentation = string.Join(VersionSeparator.ToString(), splitStringRepresentation) : 
                    stringRepresentation;
            }
        }
        
        #endregion

        
        
        #region Class lifecycle

        public ExtendedVersion(params int[] versionNumbers)
        {
            if (versionNumbers.Length == 0)
            {
                throw new ArgumentException("At least one version number must be specified!");
            }
            foreach (int versionNumber in versionNumbers)
            {
                if (versionNumber < 0)
                {
                    throw new ArgumentOutOfRangeException(
                        nameof(versionNumbers), 
                        $"Negative version numbers like {versionNumber} are not allowed!");
                }
            }
            
            intRepresentation = versionNumbers;
            splitStringRepresentation = intRepresentation.Select(subversion => subversion.ToString()).ToArray();
        }
        
        
        public ExtendedVersion(string version)
        {
            if (string.IsNullOrWhiteSpace(version))
            {
                throw new ArgumentException("Empty version string is not allowed!");
            }
            string[] subversions = version.Split(VersionSeparator);
            intRepresentation = new int[subversions.Length];
            splitStringRepresentation = new string[subversions.Length];
            for (int i = 0; i < subversions.Length; i++)
            {
                string subversion = subversions[i];
                
                if (!int.TryParse(subversion, out int result))
                {
                    throw new ArgumentException($"Can't parse version {subversion}!");
                }
                if (result < 0)
                {
                    throw new ArgumentOutOfRangeException(
                        nameof(subversion), 
                        $"Negative version numbers like {subversion} are not allowed!");
                }
                intRepresentation[i] = result;
                splitStringRepresentation[i] = subversion;
            }
        }
        
        #endregion
        
        
        
        #region IComparable implementation

        public int CompareTo(object obj)
        {
            if (obj == null)
            {
                return 1;
            }
            
            ExtendedVersion version = obj as ExtendedVersion;
            if (version == null)
            {
                throw new ArgumentException($"Argument must be {typeof(ExtendedVersion)} instance!");
            }
            
            return CompareTo(version);
        }
        
        
        public int CompareTo(ExtendedVersion version)
        {
            // In general we assume, that if one version string is substring for another version string,
            // then another version greater (e.g, 2.010 > 2.01; 2.1.3 > 2.1)
            int minLength = Math.Min(splitStringRepresentation.Length, version.splitStringRepresentation.Length);
            for (int i = 0; i < minLength; i++)
            {
                if (intRepresentation[i] < version.intRepresentation[i])
                {
                    return -1;
                }
                if (intRepresentation[i] > version.intRepresentation[i])
                {
                    return 1;
                }
                
                string subversion1 = splitStringRepresentation[i];
                string subversion2 = version.splitStringRepresentation[i];
                if (subversion1.Equals(subversion2, StringComparison.InvariantCulture))
                {
                    continue;
                }
                
                int minSubversionLength = Math.Min(subversion1.Length, subversion2.Length);
                for (int j = 0; j < minSubversionLength; j++)
                {
                    // Greater numbers have greater char code
                    if (subversion1[j] < subversion2[j])
                    {
                        return -1;
                    }
                    if (subversion1[j] > subversion2[j])
                    {
                        return 1;
                    }
                }
                
                if (subversion1.Length < subversion2.Length)
                {
                    return -1;
                }
                if (subversion1.Length > subversion2.Length)
                {
                    return 1;
                }
            }
            if (splitStringRepresentation.Length < version.splitStringRepresentation.Length)
            {
                return -1;
            }
            if (splitStringRepresentation.Length > version.splitStringRepresentation.Length)
            {
                return 1;
            }
            
            return 0;
        }
        
        #endregion
        
        
        
        #region Operators override
        
        public static bool operator ==(ExtendedVersion v1, ExtendedVersion v2) 
        {
            return v1?.Equals(v2) ?? ReferenceEquals(v2, null);
        }


        public static bool operator !=(ExtendedVersion v1, ExtendedVersion v2)
        {
            return !(v1 == v2);
        }
        
        
        public static bool operator <(ExtendedVersion v1, ExtendedVersion v2) 
        {
            if (v1 == null)
            {
                throw new ArgumentNullException(nameof(v1));
            }
            return v1.CompareTo(v2) < 0;
        }
        
        
        public static bool operator >(ExtendedVersion v1, ExtendedVersion v2) 
        {
            return v2 < v1;
        }
        
        
        public static bool operator <=(ExtendedVersion v1, ExtendedVersion v2) 
        {
            if (v1 == null)
            {
                throw new ArgumentNullException(nameof(v1));
            }
            return v1.CompareTo(v2) <= 0;
        }
        
        
        public static bool operator >=(ExtendedVersion v1, ExtendedVersion v2) 
        {
            return v2 <= v1;
        }
        
        #endregion
        
        
        
        #region IEquatable implementation

        public bool Equals(ExtendedVersion version)
        {
            if (version == null)
            {
                return false;
            }
            if (splitStringRepresentation.Length != version.splitStringRepresentation.Length)
            {
                return false;
            }

            for (int i = 0; i < splitStringRepresentation.Length; i++)
            {
                if (!splitStringRepresentation[i].Equals(version.splitStringRepresentation[i], StringComparison.InvariantCulture))
                {
                    return false;
                }
            }
            
            return true;
        }

        #endregion
        
        
        
        #region IClonable implementation
        
        public object Clone() => new ExtendedVersion(intRepresentation);
        
        #endregion
        
        
        
        #region Object overrides

        public override string ToString()
        {
            return StringRepresentation;
        }


        public override bool Equals(object obj)
        {
            return Equals(obj as ExtendedVersion);
        }


        public override int GetHashCode()
        {
            int result = 0;
            foreach (string s in splitStringRepresentation)
            {
                result += s.GetHashCode();
            }
            
            return result;
        }
        
        #endregion
    }
}
