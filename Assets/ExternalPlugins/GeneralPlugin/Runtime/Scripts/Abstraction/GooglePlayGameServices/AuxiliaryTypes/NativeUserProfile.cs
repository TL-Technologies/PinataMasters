namespace Modules.General.Abstraction.GooglePlayGameServices
{
    public struct NativeUserProfile
    {
        public string id;
        public string name;
        public string displayName;
        public string title;
        public string iconImageUri;
        public string hiResImageUri;
        
        
        public override string ToString() =>
            $"[NativeUserProfile] {nameof(id)} = {id}, {nameof(name)} = {name}, {nameof(displayName)} = {displayName}, " +
            $"{nameof(title)} = {title}, {nameof(iconImageUri)} = {iconImageUri}, {nameof(hiResImageUri)} = {hiResImageUri}.";
    }
}
