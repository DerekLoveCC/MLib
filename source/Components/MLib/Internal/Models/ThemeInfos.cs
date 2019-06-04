﻿namespace MLib.Internal.Models
{
    using Interfaces;
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Manages a set of theminfo entries.
    /// </summary>
    internal class ThemeInfos : IThemeInfos
    {
        private Dictionary<string, ThemeInfo> mDic = new Dictionary<string, ThemeInfo>();

        /// <summary>
        /// Add another theme entry by its name and Uri source.
        /// </summary>
        /// <param name="name">The unique (display) name for this theme.</param>
        /// <param name="themeSources">List of Uri based resources to be loaded for this theme.</param>
        public void AddThemeInfo(string name, List<Uri> themeSources)
        {
            mDic.Add(name, new ThemeInfo(name, themeSources));
        }

        /// <summary>
        /// Add another theme entry by its name and Uri source.
        /// </summary>
        /// <param name="theme">The <see cref="IThemeInfo"/> based object instance containing
        /// the unique name definition and collection of Uri based resources to be loaded for
        /// this theme.</param>
        public void AddThemeInfo(IThemeInfo theme)
        {
            mDic.Add(theme.DisplayName, new ThemeInfo(theme));
        }

        /// <summary>
        /// Retrieve an existing theme entry by its Uri source.
        /// Returns null if theme is not present.
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public IThemeInfo GetThemeInfo(string name)
        {
            ThemeInfo ret = null;
            mDic.TryGetValue(name, out ret);

            return ret;
        }

        /// <summary>
        /// Remove an existing theme entry by its Uri source.
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public IThemeInfo RemoveThemeInfo(string name)
        {
            ThemeInfo ret = null;
            if (mDic.TryGetValue(name, out ret) == true)
            {
                mDic.Remove(name);
                return ret;
            }

            return ret;
        }

        /// <summary>
        /// Remove all existing theme entries.
        /// </summary>
        public void RemoveAllThemeInfos()
        {
            mDic.Clear();
        }

        /// <summary>
        /// Enumerate through all themes
        /// </summary>
        /// <returns></returns>
        public IEnumerable<IThemeInfo> GetThemeInfos()
        {
            foreach (var item in mDic.Values)
                yield return item;
        }
    }
}
