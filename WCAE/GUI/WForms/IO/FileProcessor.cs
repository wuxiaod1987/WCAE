namespace WCAE.WForms.IO
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using WCAE.WForms.IO;
    using WCAE.WForms.IO.Formats;
    using TriangleNet.IO;
    using TriangleNet.Geometry;
    using TriangleNet;

    /// <summary>
    /// Provides static methods to read and write mesh files.
    /// </summary>
    public static class FileProcessor
    {
        static Dictionary<string, ITriangleFiles> container = new Dictionary<string, ITriangleFiles>();

        public static bool CanHandleFile(string path)
        {
            if (File.Exists(path))
            {
                var provider = GetProviderInstance(path);

                if (provider != null)
                {
                    return true;
                }
            }

            return false;
        }

        private static ITriangleFiles GetProviderInstance(string path)
        {
            string ext = Path.GetExtension(path);

            ITriangleFiles provider = null;

            if (container.ContainsKey(ext))
            {
                provider = container[ext];
            }
            else
            {
                provider = CreateProviderInstance(ext);
            }

            return provider;
        }

        private static ITriangleFiles CreateProviderInstance(string ext)
        {
            // TODO: automate by using IMeshFormat's Extensions property.

            ITriangleFiles provider = null;

            if (ext == ".node" || ext == ".poly" || ext == ".ele")
            {
                provider = new TriangleFile();
            }

            if (provider == null)
            {
                throw new NotImplementedException("File format not implemented.");
            }

            container.Add(ext, provider);

            return provider;
        }

        #region "Triangle数据的IO"
        /// <summary>
        /// Returns true, if the given file contains mesh information.
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static bool ContainsMeshData(string path)
        {
            ITriangleFiles provider = GetProviderInstance(path);

            return provider.ContainsMeshData(path);
        }

        /// 读入Triangle的几何数据
        /// <summary>
        /// Read an input geometry from given file.
        /// </summary>
        public static InputGeometry Read(string path)
        {
            ITriangleFiles provider = GetProviderInstance(path);

            return provider.Read(path);
        }

        /// 读入Trianglede的Mesh数据 
        /// <summary>
        /// Read a mesh from given file.
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static Mesh Import(string path)
        {
            ITriangleFiles provider = GetProviderInstance(path);

            return provider.Import(path);
        }

        /// 保存Triangle的Mesh数据
        /// <summary>
        /// Save the current mesh to given file.
        /// </summary>
        public static void Save(string path, Mesh mesh)
        {
            ITriangleFiles provider = GetProviderInstance(path);

            provider.Write(mesh, path);
        }
        #endregion
    }
}
