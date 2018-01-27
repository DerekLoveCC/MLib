﻿namespace FolderBrowser.ViewModels
{
    using FileSystemModels.Models;
    using FileSystemModels.Models.FSItems;
    using FolderBrowser.Interfaces;
    using InplaceEditBoxLib.ViewModels;
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Windows;
    using System.Windows.Media.Imaging;

    /// <summary>
    /// Implment the viewmodel for one folder entry for a collection of folders.
    /// </summary>
    internal class ItemViewModel : EditInPlaceViewModel, IItemViewModel
    {
        #region fields
        private static readonly IItemViewModel DummyChild = new ItemViewModel();
        private bool _IsSelected;
        private bool _IsExpanded;

        private PathModel _Model;

        private readonly SortableObservableDictionaryCollection _Folders;
        private readonly IItemViewModel _Parent;
        private string _VolumeLabel;

        private object _LockObject = new object();
        #endregion fields

        #region constructor
        /// <summary>
        /// Construct an item viewmodel from a path.
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public ItemViewModel(PathModel model, IItemViewModel parent)
            : this()
        {
            _Parent = parent;
            _Folders.AddItem(DummyChild);
            _Model = new PathModel(model);

            // Names of Logical drives cannot be changed with this
            if (_Model.PathType == FSItemType.LogicalDrive)
                this.IsReadOnly = true;
        }

        protected ItemViewModel(string dir, FSItemType ItemType, IItemViewModel parent)
           : this(new PathModel(dir, ItemType), parent)
        {
        }

        /// <summary>
        /// Standard <seealso cref="ItemViewModel"/> constructor
        /// </summary>
        protected ItemViewModel()
          : base()
        {
            _IsExpanded = _IsSelected = false;

            _Model = null;

            // Add dummy folder by default to make tree view show expander by default
            _Folders = new SortableObservableDictionaryCollection();

            _VolumeLabel = null;
        }
        #endregion constructor

        #region properties
        /// <summary>
        /// Gets the name of this folder (without its root path component).
        /// </summary>
        public string ItemName
        {
            get
            {
                if (_Model != null)
                    return _Model.Name;

                return string.Empty;
            }
        }

        /// <summary>
        /// Get/set file system Path for this folder.
        /// </summary>
        public string ItemPath
        {
            get
            {
                if (_Model != null)
                    return _Model.Path;

                return string.Empty;
            }
        }

        /// <summary>
        /// Gets the parent object where this object is the child in the treeview.
        /// </summary>
        public IItemViewModel Parent
        {
            get
            {
                return _Parent;
            }
        }

        /// <summary>
        /// Gets a folder item string for display purposes.
        /// This string can evaluete to 'C:\ (Windows)' for drives,
        /// if the 'C:\' drive was named 'Windows'.
        /// </summary>
        public string DisplayItemString
        {
            get
            {
                switch (ItemType)
                {
                    case FSItemType.LogicalDrive:
                        try
                        {
                            if (_VolumeLabel == null)
                            {
                                DriveInfo di = new System.IO.DriveInfo(ItemName);

                                if (di.IsReady == true)
                                    _VolumeLabel = di.VolumeLabel;
                                else
                                    return string.Format("{0} ({1})", ItemName, "device is not ready");
                            }

                            return string.Format("{0} {1}", ItemName, (string.IsNullOrEmpty(_VolumeLabel)
                                                                                ? string.Empty
                                                                                : string.Format("({0})", _VolumeLabel)));
                        }
                        catch (Exception exp)
                        {
                            ////base.ShowNotification("DriveInfo cannot be optained for:" + FolderName, FileSystemModels.Local.Strings.STR_MSG_UnknownError);

                            // Just return a folder name if everything else fails (drive may not be ready etc).
                            return string.Format("{0} ({1})", ItemName, exp.Message.Trim());
                        }

                    case FSItemType.Folder:
                    case FSItemType.Unknown:
                    default:
                        return ItemName;
                }
            }
        }

        /// <summary>
        /// Get/set observable collection of sub-folders of this folder.
        /// </summary>
        public IEnumerable<IItemViewModel> Folders
        {
            get
            {
                return _Folders;
            }
        }

        /// <summary>
        /// Get/set whether this folder is currently selected or not.
        /// </summary>
        public bool IsSelected
        {
            get
            {
                return _IsSelected;
            }

            set
            {
                if (_IsSelected != value)
                {
                    Logger.Debug("Detail: set Folder IsSelected");

                    _IsSelected = value;

                    RaisePropertyChanged(() => IsSelected);

                    //// if (value == true)
                    ////    IsExpanded = true;                 // Default windows behaviour of expanding the selected folder
                }
            }
        }

        /// <summary>
        /// Get/set whether this folder is currently expanded or not.
        /// </summary>
        public bool IsExpanded
        {
            get
            {
                return _IsExpanded;
            }

            set
            {
                if (_IsExpanded != value)
                {
                    _IsExpanded = value;

                    RaisePropertyChanged(() => IsExpanded);
                }
            }
        }

        /// <summary>
        /// Gets the type of this item (eg: Folder, HardDisk etc...).
        /// </summary>
        public FSItemType ItemType
        {
            get
            {
                if (_Model != null)
                    return _Model.PathType;

                return FSItemType.Unknown;
            }
        }

        /// <summary>
        /// Determine whether child is a dummy (must be evaluated and replaced
        /// with real data) or not.
        /// </summary>
        public bool HasDummyChild
        {
            get
            {
                if (this._Folders != null)
                {
                    if (this._Folders.Count == 1)
                    {
                        if (this._Folders[0] == DummyChild)
                            return true;
                    }
                }

                return false;
            }
        }
        #endregion properties

        #region methods
        /// <summary>
        /// Adds the folder item into the collection of sub-folders of this folder.
        /// </summary>
        /// <param name="item"></param>
        public void ChildAdd(IItemViewModel item)
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                _Folders.AddItem(item);
            });
        }

        /// <summary>
        /// Renames a child below this item.
        /// </summary>
        /// <param name="oldName"></param>
        /// <param name="newName"></param>
        /// <returns></returns>
        public bool ChildRename(string oldName, string newName)
        {
            var item = ChildTryGet(oldName);

            if (item == null)
                return false;

            _Folders.RenameItem(item, newName);

            return true;
        }

        /// <summary>
        /// Rename the name of the folder into a new name.
        /// </summary>
        /// <param name="newFolderName"></param>
        public void Rename(string newFolderName)
        {
            Logger.DebugFormat("Detail: Rename into new folder {0}:", newFolderName);

            lock (_LockObject)
            {
                try
                {
                    if (newFolderName != null)
                    {
                        PathModel sourceDir = new PathModel(ItemPath, FSItemType.Folder);
                        PathModel newFolderPath;

                        if (PathModel.RenameFileOrDirectory(sourceDir, newFolderName, out newFolderPath) == true)
                        {
                            ResetModel(newFolderPath);
                        }
                    }
                }
                catch (Exception exp)
                {
                    this.ShowNotification(FileSystemModels.Local.Strings.STR_RenameFolderErrorTitle, exp.Message);
                }
                finally
                {
                    RaisePropertyChanged(() => ItemName);
                    RaisePropertyChanged(() => ItemPath);
                    RaisePropertyChanged(() => DisplayItemString);
                }
            }
        }

        /// <summary>
        /// Attempts to find an item with the given name in the list of child items
        /// below this item and returns it or null.
        /// </summary>
        /// <param name="folderName"></param>
        /// <returns></returns>
        public IItemViewModel ChildTryGet(string folderName)
        {
            if (HasDummyChild == true)
                return null;

            return _Folders.TryGet(folderName);
        }

        internal static void AddFolder(ItemViewModel f, ItemViewModel item)
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                f.ChildAdd(item);
            });
        }

        /// <summary>
        /// Construct a <seealso cref="FolderViewModel"/> item that represents a Windows
        /// file system drive object (eg.: 'C:\').
        /// </summary>
        /// <param name="driveLetter"></param>
        /// <returns></returns>
        internal static FolderViewModel ConstructDriveFolderViewModel(string driveLetter)
        {
            return new FolderViewModel(new PathModel(driveLetter, FSItemType.LogicalDrive), null);
        }

        /// <summary>
        /// Load all sub-folders into the Folders collection.
        /// </summary>
        public virtual void LoadFolders()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Create a new folder with a standard name
        /// 'New folder n' underneath this folder.
        /// </summary>
        /// <returns>a viewmodel of the newly created directory or null</returns>
        public virtual IItemViewModel CreateNewDirectory()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Remove all sub-folders from a given folder.
        /// </summary>
        public void ClearFolders()
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                _Folders.Clear();
            });
        }

        /// <summary>
        /// Method executes when item is renamed
        /// -> model name is required to be renamed and dependend
        /// properties are updated.
        /// </summary>
        /// <param name="model"></param>
        private void ResetModel(PathModel model)
        {
            var oldModel = _Model;

            _Model = new PathModel(model);

            if (oldModel == null && model == null)
                return;

            if (oldModel == null && model != null || oldModel != null && model == null)
            {
                RaisePropertyChanged(() => ItemType);
                RaisePropertyChanged(() => ItemName);
                RaisePropertyChanged(() => ItemPath);

                return;
            }

            if (oldModel.PathType != _Model.PathType)
                RaisePropertyChanged(() => ItemType);

            if (string.Compare(oldModel.Name, _Model.Name, true) != 0)
                RaisePropertyChanged(() => ItemName);

            if (string.Compare(oldModel.Path, _Model.Path, true) != 0)
                RaisePropertyChanged(() => ItemPath);
        }

        /// <summary>
        /// Shows a pop-notification message with the given title and text.
        /// </summary>
        /// <param name="title"></param>
        /// <param name="message"></param>
        /// <param name="imageIcon"></param>
        /// <returns>true if the event was succesfully fired.</returns>
        public new bool ShowNotification(string title, string message,
                                         BitmapImage imageIcon = null)
        {
            try
            {
                Application.Current.Dispatcher.Invoke(() =>
                {
                    return base.ShowNotification(title, message, imageIcon);
                });
            }
            catch
            {
            }

            return false;
        }
        #endregion methods
    }
}
