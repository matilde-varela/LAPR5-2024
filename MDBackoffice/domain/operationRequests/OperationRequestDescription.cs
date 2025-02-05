using MDBackoffice.Domain.Shared;

namespace MDBackoffice.Domain.OperationRequests
{
    public class OperationRequestDescription : IValueObject
    {
        public string DescriptionText { get; set; }

        public OperationRequestDescription()
        {
            // for ORM
        }

        public OperationRequestDescription(string text)
        {
            if (string.IsNullOrWhiteSpace(text) || string.IsNullOrEmpty(text.Trim()))
            {
                throw new BusinessRuleValidationException("Operation Request's description cannot be null or empty.");
            }
            this.DescriptionText = text;
        }

        public override bool Equals(object obj)
        {
            if (obj == null || GetType() != obj.GetType())
            {
                return false;
            }

            var other = (OperationRequestDescription)obj;
            return DescriptionText == other.DescriptionText;
        }

        public override int GetHashCode()
        {
            return DescriptionText.GetHashCode();
        }

        public override string ToString()
        {
            return DescriptionText;
        }
    }
}