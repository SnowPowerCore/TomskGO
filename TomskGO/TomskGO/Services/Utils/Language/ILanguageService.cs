﻿using System.Globalization;

namespace TomskGO.Core.Services.Utils.Language
{
    public interface ILanguageService
    {
        string Current { get; }

        string Culture { get; }

        void DetermineAndSetLanguage();

        void SetLanguage(CultureInfo lang);
    }
}