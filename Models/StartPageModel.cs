namespace FiveInARowWeb.Models {
    public static class StartPageModel {
        public class StartGameValueModel {
            public required string PassCode { get; set; }
            public required string Team {  get; set; }
            public required string Uname { get; set; }
        }
    }
}
