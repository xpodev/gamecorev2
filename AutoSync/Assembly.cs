using ILRepacking;

namespace GameCore.Net.Sync
{
    public class Assembly
    {
        public string AssemblyPath { get; }

        public Assembly(string assemblyPath)
        {
            AssemblyPath = assemblyPath;
        }

        public void Repack(string outputPath)
        {
            RepackOptions options = new RepackOptions(new string[] {
                outputPath, AssemblyPath
            })
            {
                OutputFile = outputPath,
                InputAssemblies = new string[] { AssemblyPath }
            };

            {
                ILRepack repack = new ILRepack(options);
                repack.Repack();
            }
        }
    }
}
