namespace IPWhistleblower.Helpers
{
    public class IP2CResponse
    {
        public string TwoLetterCode { get; set; }
        public string ThreeLetterCode { get; set; }
        public string CountryName { get; set; }



        public static IP2CResponse FromResponseString(string responseString)
        {
            var parts = responseString.Split(';');

            if (parts.Length != 4)
            {
                throw new FormatException("Unexpected response format.");
            }

            return new IP2CResponse
            {
                TwoLetterCode = parts[1],
                ThreeLetterCode = parts[2],
                CountryName = parts[3]
            };
        }
    }
}
