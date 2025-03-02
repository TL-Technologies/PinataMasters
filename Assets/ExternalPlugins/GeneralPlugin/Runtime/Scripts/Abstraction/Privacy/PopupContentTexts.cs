namespace Modules.General.Abstraction.Privacy
{
    public class PopupContentTexts
    {
        #region Properties

        public string Header { get; set; }
        
        
        public string Message { get; set; }
        
        
        public string FirstButtonText { get; set; }
        
        
        public string SecondButtonText { get; set; }

        #endregion


        
        #region Class lifecycle

        public PopupContentTexts(string header, string message, string firstButtonText, string secondButtonText)
        {
            UpdateAll(header, message, firstButtonText, secondButtonText);
        }

        
        public PopupContentTexts() { }

        
        public PopupContentTexts WithHeader(string header)
        { 
            Header = header;

            return this;
        }

        
        public PopupContentTexts WithMessage(string message)
        {
            Message = message;

            return this;
        }

        
        public PopupContentTexts WithFirstButtonText(string firstButtonText)
        {
            FirstButtonText = firstButtonText;

            return this;
        }

        
        public PopupContentTexts WithSecondButtonText(string secondButtonText)
        {
            SecondButtonText = secondButtonText;

            return this;
        }

        #endregion



        #region Methods

        public void UpdateAll(string header, string message, string firstButtonText, string secondButtonText)
        {
            Header = header;
            Message = message;
            FirstButtonText = firstButtonText;
            SecondButtonText = secondButtonText;
        }

        #endregion
    }
}