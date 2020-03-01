using AsyncAwaitBestPractices.MVVM;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using TomskGO.Core.Resources;
using TomskGO.Core.Services.Utils.Language;
using TomskGO.Core.Services.Utils.Message;
using TomskGO.Models.Utils;
using XF.Material.Forms.UI.Dialogs;

namespace TomskGO.Core.ViewModels.Utils
{
    /// <summary>
    ///     Simple language control viewmodel
    /// </summary>
    internal class LanguageViewModel : BaseViewModel
    {
        #region Fields
        private readonly ILanguageService _language;
        private readonly IMessageService _message;
        #endregion

        #region Properties
        public string CurrentLanguage => _language.Current;

        public CultureModel CurrentModel =>
            LanguagesList.FirstOrDefault(x => x.Culture == CultureInfo.GetCultureInfo(CurrentLanguage));

        public string CurrentDisplayLanguage =>
            CurrentModel.LanguageName;

        /// <summary>
        ///     List of supported app languages
        /// </summary>
        public List<CultureInfo> Languages => new List<CultureInfo>
        {
            CultureInfo.GetCultureInfo("ru"),
            CultureInfo.GetCultureInfo("en"),
        };

        public List<CultureModel> LanguagesList => new List<CultureModel>()
        {
            new CultureModel { Culture = CultureInfo.GetCultureInfo("ru"), LanguageName="Русский" },
            new CultureModel { Culture = CultureInfo.GetCultureInfo("en"), LanguageName="English" },
        };
        #endregion

        #region Commands
        public IAsyncCommand ChangeLanguageCommand =>
            new AsyncCommand(ChangeLanguageAsync, continueOnCapturedContext: true);
        #endregion

        #region Constructor
        public LanguageViewModel(ILanguageService language,
                                 IMessageService message)
        {
            _language = language;
            _message = message;
        }
        #endregion

        #region Methods
        private Task ChangeLanguageAsync()
        {
            var options = LanguagesList.Select(i => i.LanguageName).ToList();
            return MaterialDialog.Instance.SelectActionAsync(options)
                .ContinueWith(t =>
                {
                    var result = t.Result;
                    if (result >= 0 && result < LanguagesList.Count)
                    {
                        _language.SetLanguage(Languages[result]);
                        return _message.DisplayInfoAsync(AppResources.langChangeInfo);
                    }
                    return Task.CompletedTask;
                }, TaskContinuationOptions.OnlyOnRanToCompletion);
        }
        #endregion
    }
}