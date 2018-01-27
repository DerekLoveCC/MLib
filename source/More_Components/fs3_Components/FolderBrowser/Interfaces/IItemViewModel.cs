﻿namespace FolderBrowser.Interfaces
{
    using System.Collections.Generic;
    using System.Windows.Media.Imaging;

    public interface IItemViewModel : IParent
    {
        #region properties
        /// <summary>
        /// Determine whether child is a dummy (must be evaluated and replaced
        /// with real data) or not.
        /// </summary>
        bool HasDummyChild { get; }

        /// <summary>
        /// Gets a string that is to be displayed for this item.
        /// This is primarily used for drives which can show more
        /// information than just their path portion ... 'C:'
        /// </summary>
        string DisplayItemString { get; }

        /// <summary>
        /// Gets the name (without the path) of this item.
        /// </summary>
        string ItemName { get; }

        /// <summary>
        /// Gets the complete path and filder name that
        /// represents the address of this folder.
        /// </summary>
        string ItemPath { get; }

        /// <summary>
        /// Gets a collection of sub-folders (if any) that are contained within this folder.
        /// </summary>
        IEnumerable<IItemViewModel> Folders { get; }

        /// <summary>
        /// Gets whether this folder is currently expanded or not.
        /// </summary>
        bool IsExpanded { get; }

        /// <summary>
        /// Get/set whether this folder is currently selected or not.
        /// </summary>
        bool IsSelected { get; }

        /// <summary>
        /// Gets the type of the underlying model that is represented by this object.
        /// The underlying can be LogicalDrive, Folder, or some other item.
        /// </summary>
        FileSystemModels.Models.FSItems.FSItemType ItemType { get; }
        #endregion properties

        #region methods
        /// <summary>
        /// Adds the folder item into the collection of sub-folders
        /// of this folder.
        /// </summary>
        /// <param name="item"></param>
        void ChildAdd(IItemViewModel item);

        /// <summary>
        /// Create a new folder with a standard name
        /// 'New folder n' underneath this folder.
        /// </summary>
        /// <returns>a viewmodel of the newly created directory or null</returns>
        IItemViewModel CreateNewDirectory();

        /// <summary>
        /// Renames a child below this item.
        /// </summary>
        /// <param name="oldName"></param>
        /// <param name="newName"></param>
        /// <returns></returns>
        bool ChildRename(string oldName, string newName);

        /// <summary>
        /// Rename the name of the folder into a new name.
        /// </summary>
        /// <param name="newFolderName"></param>
        void Rename(string newFolderName);

        /// <summary>
        /// Remove all sub-folders from a given folder.
        /// </summary>
        void ClearFolders();

        /// <summary>
        /// Attempts to find an item with the given name in the list of child items
        /// below this item and returns it or null.
        /// </summary>
        /// <param name="folderName"></param>
        /// <returns></returns>
        IItemViewModel ChildTryGet(string folderName);

        /// <summary>
        /// Shows a pop-notification message with the given title and text.
        /// </summary>
        /// <param name="title"></param>
        /// <param name="message"></param>
        /// <param name="imageIcon"></param>
        /// <returns>true if the event was succesfully fired.</returns>
        bool ShowNotification(string title, string message,
                              BitmapImage imageIcon = null);

        #endregion methods
    }
}
