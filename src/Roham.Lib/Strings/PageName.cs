using System;

namespace Roham.Lib.Strings
{
    [Serializable]
    public sealed class PageName : EnforcedString, IEquatable<PageName>
    {
        public PageName(string value) : base(value) { }

        protected override string Correct(string value)
        {
            return value.Slugify();
        }

        public static implicit operator PageName(string value)
        {
            return new PageName((value ?? string.Empty).Trim());
        }

        public static implicit operator string (PageName name)
        {
            if (name == null)
                return null;
            return name.ToString();
        }

        public static bool operator ==(PageName left, PageName right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(PageName left, PageName right)
        {
            return !Equals(left, right);
        }

        public bool Equals(PageName other)
        {
            return Equals((EnforcedString)other);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            return Equals(obj as PageName);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
}