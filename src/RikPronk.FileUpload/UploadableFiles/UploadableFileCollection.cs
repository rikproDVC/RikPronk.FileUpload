using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Web;

namespace RikPronk.FileUpload.Core
{
    /// <summary>
    /// Collection of UploadableFiles which assures unique file names for all UploadableFiles added. <para />
    /// By default, the UploadableFileCollection will resolve duplicate file names upon addition as "{Filename} ({number of duplicates})", identical to Windows. <para />
    /// The resolver can be modified with the SetFileNameResolver method. 
    /// </summary>
    public class UploadableFileCollection : Collection<IUploadableFile>
    {
        /// <summary>
        /// Collection of UploadableFiles which assures unique file names for all UploadableFiles added. <para />
        /// By default, the UploadableFileCollection will resolve duplicate file names upon addition as "{Filename} ({number of duplicates})", identical to Windows. <para />
        /// The resolver can be modified with the SetFileNameResolver method. 
        /// </summary>
        public UploadableFileCollection()
        {
            _saveNames = new List<string>();
            _resolveSaveName = delegate (string s, int i)
            {
                return string.Format("{0} ({1})", s, i);
            };
        }

        /// <summary>
        /// Sets the name resolver in case of a save name conflict. By default, the resolver will resolve duplicate file names upon addition as "{Filename} ({number of duplicates})", identical to Windows.
        /// </summary>
        /// <param name="value">The function to resolve the file name with</param>
        public void SetSaveNameResolver(Func<string, int, string> value)
        {
            _resolveSaveName = value;
        }

        /// <summary>
        /// Creates a new UploadableFileCollection directly from an IEnumerable. File names are directly copied from the uploaded files and assured to be unique inside the collection's items.
        /// </summary>
        /// <typeparam name="T">The output UploadableFile, derived from IUploadableFile</typeparam>
        /// <param name="files">The source IEnumerable</param>
        /// <returns>
        /// A new UploadableFileCollection
        /// </returns>
        public static UploadableFileCollection From<T>(IEnumerable<HttpPostedFileBase> files) where T: IUploadableFile
        {
            return From<T>(files, delegate (string fileName)
            {
                return fileName;
            });
        }

        /// <summary>
        /// Creates a new UploadableFileCollection directly from an IEnumerable. File names are directly copied from the uploaded files and assured to be unique inside the collection's items.
        /// </summary>
        /// <typeparam name="T">The output UploadableFile, derived from IUploadableFile</typeparam>
        /// <param name="files">The source IEnumerable</param>
        /// <param name="saveNameGenerator">Function to generate the name the file will be saved as.<para/>
        ///  Parameter string is the original file name as uploaded by the user. Returns the savename of the file</param>
        /// <returns>
        /// A new UploadableFileCollection
        /// </returns>
        public static UploadableFileCollection From<T>(IEnumerable<HttpPostedFileBase> files, Func<string, string> saveNameGenerator) where T : IUploadableFile
        {
            var c = new UploadableFileCollection();
            foreach (var file in files)
            {
                var saveName = saveNameGenerator(file.FileName);
                var instance = (T)Activator.CreateInstance(typeof(T), new object[] { file, saveName });
                c.Add(instance);
            }

            return c;
        }

        /// <summary>
        /// Asserts if adding a new file will not cause file name conflicts.
        /// </summary>
        /// <param name="insertedName">Name of the filename to be inserted</param>
        /// <returns>True if file names remain unique, false if the file name duplicates an already existing file name</returns>
        public bool AssertUniqueSaveName(string insertName)
        {
            return !_saveNames.Any(n => n == insertName);
        }

        /// <summary>
        /// Asserts that the file names in the UploadableFileCollection will not conflict with existing files in the source to be saved to.
        /// </summary>
        /// <param name="existingNames">Array of filenames in the source to be saved to</param>
        /// <returns>True if file names remain unique, false if any of the file names duplicates an already existing file name</returns>
        public bool AssertUniqueSaveNames(string[] existingNames)
        {
            foreach (var fileName in existingNames)
            {
                if (!AssertUniqueSaveName(fileName))
                {
                    return false;
                }
            }
            return true;
        }

        /// <summary>
        /// Asserts that the file names in the UploadableFileCollection will not conflict with existing files in the source to be saved to, and resolves these file names directly if needed. <para/>
        /// Uses the FileNameResolver to resolve file names.<para/>
        /// This method is commonly called from Uploaders to assure the files being uploaded won't conflict with existing files.
        /// </summary>
        /// <param name="existingNames">Array of filenames in the source to be saved to</param>
        public void AssertAndResolveUniqueSaveNames(string[] existingNames)
        {
            for (var i = 0; i < Items.Count; i++)
            {
                if (existingNames.Any(n => n == Items[i].SaveName))
                {
                    var n = 1;
                    var solution = ResolveSaveName(Items[i].SaveName, n);
                    n++;

                    while (!AssertUniqueSaveName(solution) || existingNames.Any(x => x == solution))
                    {
                        solution = ResolveSaveName(Items[i].SaveName, n);
                        n++;
                    }

                    Items[i].SaveName = solution;
                }
            }
        }

        /// <summary>
        /// Inserts an element into the <see cref="T:System.Collections.ObjectModel.Collection`1" /> at the specified index and assures it will have a unique filename inside the collection.
        /// </summary>
        /// <param name="index">The zero-based index at which <paramref name="item" /> should be inserted.</param>
        /// <param name="item">The object to insert. The value can be null for reference types.</param>
        protected override void InsertItem(int index, IUploadableFile item)
        {
            item.SaveName = AssertAndResolveUniqueSaveName(item.SaveName);
            base.InsertItem(index, item);
            _saveNames.Add(item.SaveName);
        }

        /// <summary>
        /// Asserts adding a new file will not cause file name conflicts and provides a replacement name if needed
        /// </summary>
        /// <param name="insertedName">Name of the filename to be inserted</param>
        /// <returns>The modified file name</returns>
        private string AssertAndResolveUniqueSaveName(string insertName)
        {
            if (!AssertUniqueSaveName(insertName))
            {
                var i = 1;
                var solution = ResolveSaveName(insertName, i);
                i++;

                while (!AssertUniqueSaveName(solution))
                {
                    solution = ResolveSaveName(insertName, i);
                    i++;
                }
                return solution;
            }

            return insertName;
        }


        /// <summary>
        /// Internal wrapper around the filename resolver provided by the developer. <para/>
        /// Seperates extension from file name before resolving and re-adds it afterwards.
        /// </summary>
        /// <param name="s">File name, including extension</param>
        /// <param name="i">Amount of duplicate files</param>
        /// <returns></returns>
        private string ResolveSaveName(string s, int i)
        {
            var name = Path.GetFileNameWithoutExtension(s);
            return string.Format("{0}{1}", _resolveSaveName(name, i), Path.GetExtension(s));
        }

        private List<string> _saveNames;
        private Func<string, int, string> _resolveSaveName;
    }
}
