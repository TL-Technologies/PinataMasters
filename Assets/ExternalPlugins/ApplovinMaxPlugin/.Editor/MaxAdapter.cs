using System.Collections.Generic;


namespace Modules.Max.Editor
{
    public class MaxAdapter
    {
        public class AndroidPackage
        {
            public string Spec { get; set; }
            public string Repository { get; set; }
        }
        
        public class IosPod
        {
            public string Name { get; set; }
            public string Version { get; set; }
            public string Source { get; set; }
        }
        
        public virtual List<AndroidPackage> AndroidPackages { get; set; }
        public virtual List<IosPod> IosPods { get; set; }
    }
}
