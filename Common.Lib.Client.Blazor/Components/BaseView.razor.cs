using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using Common.Lib.Infrastructure.Actions;
using Common.Lib.Client.UI;

namespace Common.Lib.Client.Components
{
    public partial class BaseView : ComponentBase
    {
        [Inject]
        public IJSRuntime JSRuntime { get; set; }

        [Inject]
        public NavigationManager NavigationManager { get; set; }

        //BabylonDataManager BabylonDataManager { get; set; }

        public Action<List<string>>? ShowErrorsAction { get; set; }

        public BaseViewModel ViewModel { get; set; }

        public bool IsLoadedAsync { get; set; }

        public bool IsWaitingResponseAsync
        {
            get
            {
                return _isWaitingResponseAsync;
            }
            set
            { 
                _isWaitingResponseAsync = value;
                StateHasChanged();
            }
        }
        bool _isWaitingResponseAsync;

        public BaseView()
        {
        }

        protected override async Task OnInitializedAsync()
        {
            //await RegisterTranslations();
			RegisterViewModel();
		}

        public async Task SendAlertAsync(string msg)
        {
            await JSRuntime.InvokeVoidAsync("alert", msg);
        }

        public async Task ShowErrorsAsync()
        {
            if (ViewModel == null)
            {
                Log.WriteLine("ViewModel is null");
            }
            if (ShowErrorsAction == null)
                await SendAlertAsync(string.Join("\n", ViewModel.Errors));
            else
                ShowErrorsAction?.Invoke(ViewModel.Errors);
        }

        public async Task<bool> CheckResultAsync(QueryResult qr, string msg)
        {
            if (!qr.IsSuccess)
                await JSRuntime.InvokeVoidAsync("alert", "error:" + msg);

            ViewModel.Errors.Clear();

            return qr.IsSuccess;
        }
        public async Task<bool> CheckResultAsync(bool result)
        {
            if (!result)
                await ShowErrorsAsync();

            return result;
        }

        public void CallAsyncAndRefresh(Func<Task> callback)
        {
            InvokeAsync(async () =>
            {
                await callback();
                StateHasChanged();
            });
        }

        public void NavigateTo(string page)
        {
            NavigationManager.NavigateTo("/"+page);
        }
		void RegisterViewModel()
		{
			var prop = this.GetType()
							.GetProperties()
							.Where(x => x.Name != "ViewModel")
							.FirstOrDefault(x => x.PropertyType.InheritsFrom<BaseViewModel>());

            if (prop != null)
                ViewModel = prop.GetValue(this) as BaseViewModel;
		}

        #region Babylon

        protected virtual async Task RegisterTranslationsAsync()
        {
            throw new NotImplementedException();
        }

        //      public void RegisterLabel(string code, string spanish, string english = "", string catalan = "") 
        //      {
        //          english = string.IsNullOrEmpty(english ) ? spanish : english;
        //          catalan = string.IsNullOrEmpty(catalan) ? spanish : catalan;

        //          BabylonDataManager.RegisterLabel(code, spanish, english, catalan);
        //      }

        //      public string GetTranslation(string code)
        //      {
        //          return BabylonDataManager.GetTranslation(code);
        //      }

        //      public string GetTranslation(string code, int languageType)
        //      {
        //          return BabylonDataManager.GetTranslation(code, languageType);
        //      }

        public async Task RegisterConceptsAsync()
        {
            throw new NotImplementedException();
            //await BabylonDataManager.RegisterConcepts();
        }

        //      public void SelectLanguage(int languageType) 
        //      {
        //          BabylonDataManager.CurrentLanguage = languageType;
        //          StateHasChanged();
        //      }

        #endregion
    }
}
