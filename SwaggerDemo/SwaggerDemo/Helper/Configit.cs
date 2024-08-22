// Filename: Configit.cs
// Author: fang
// Created: August 20, 2024
// Description: This class will use CheckDependencies from Configit 
//              

using SwaggerDemo.Model;
using Configit.Assignment.CheckDependencies;
using Configit.Assignment.CheckDependencies.Data;

namespace SwaggerDemo.Helper
{
    public class Configit
    {
        readonly static private string _packagelist = "A,1:A,2:B,1:B,3:C,2:D,1";
        readonly static private DependenciesChecker _dependenciesChecker= new DependenciesChecker(BuildPackageRepro());
        private static PackageRepository BuildPackageRepro()
        {
            PackageRepository _packageRepository = new PackageRepository();

            //Rules Dependencies:
            //a1 requires c1+b1
            VersionedPackage a1 = new VersionedPackage("A", "1");
            VersionedPackage c1 = new VersionedPackage("C", "1");
            VersionedPackage b1 = new VersionedPackage("B", "1");

            _packageRepository.AddVersionedPackage(a1);
            _packageRepository.AddVersionedPackage(c1);
            _packageRepository.AddVersionedPackage(b1);

            _packageRepository.AddDependency(a1, c1);
            _packageRepository.AddDependency(a1, b1);

            //c2 requires d1
            VersionedPackage c2 = new VersionedPackage("C", "2");
            VersionedPackage d1 = new VersionedPackage("D", "1");
            _packageRepository.AddVersionedPackage(c2);
            _packageRepository.AddVersionedPackage(d1);
            _packageRepository.AddDependency(c2, d1);

            //a2 requires c2
            VersionedPackage a2 = new VersionedPackage("A", "2");
            //VersionedPackage c2 = new VersionedPackage("C", "2");
            _packageRepository.AddVersionedPackage(a2);
            _packageRepository.AddDependency(a2, c2);

            return _packageRepository;
        }
        public static string Verify(string packageAB)
        {
            string packageA = packageAB.Split(':')[0];
            string packageB = packageAB.Split(':')[1];
            string[] pA = packageA.Split(',');
            string[] pB = packageB.Split(',');
            bool result = _dependenciesChecker.Validate(new List<VersionedPackage> { new VersionedPackage(pA[0], pA[1]), new VersionedPackage(pB[0], pB[1]) });
            string hints = $"[ package_list ='{_packagelist}' ]";
            if (!_packagelist.Contains(packageA)) return result + hints;
            if (!_packagelist.Contains(packageB)) return result + hints;
            return result.ToString();
        }
        public static PackageInfo VerifyPackage(string packageAB)
        {
            List<VersionedPackage> packageConf = new List<VersionedPackage>();
            string packages = "";
            foreach (string package in packageAB.Split(':'))
            {   
                if(!package.Contains(",")) continue;
                if (packages.Length == 0) packages = package;
                else packages += ":"+ package;
                string name = package.Split(',')[0];
                string version = package.Split(',')[1];
                packageConf.Add(new VersionedPackage(name, version));
            }
            bool result = _dependenciesChecker.Validate(packageConf);
            PackageInfo packageInfo = new PackageInfo();
            packageInfo.package = packages;
            packageInfo.result = result.ToString(); ;
            packageInfo.list = _packagelist;
            return packageInfo;
        }
    }
}
