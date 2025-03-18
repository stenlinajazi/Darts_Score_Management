namespace Darts_Score_Management.CustomExceptions
{
    public class GameRuleViolationException : Exception
    {
        public string RuleViolated { get; }

        public GameRuleViolationException(string message, string ruleViolated)
            : base(message)
        {
            RuleViolated = ruleViolated;
        }
    }   
}
