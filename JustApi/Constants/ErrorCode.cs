namespace JustApi.Constant
{
    public class ErrorCode
    {
        public static int ESuccess = 200;

        public static int EGeneralError = 1000;
        public static int EUnknownError = 1001;
        public static int EParameterError = 1002;
        public static int EResourceNotFoundError = 1003;

        // User error
        public static int ECredentialError = 2000;
        public static int ETokenError = 2001;
        public static int ETokenExpired = 2002;
        public static int EMultipleLogin = 2003;
        public static int EAccountDisabled = 2004;
        public static int EAccountDeleted = 2005;
        public static int EUserNotFound = 2006;
        public static int EUserAlreadyExisted = 2007;
        public static int EUserPasswordError = 2008;
        public static int ERetryTime = 2009;
        public static int EInvalidOtp = 2010;
        public static int EInvalidRefNum = 2011;
        public static int EOtpExpired = 2012;


        // Company error
        public static int ECompanyDeleted = 2100;
        public static int ECompanyNotFound = 2101;
        public static int ECompanyDisabled = 2102;

        // Job error
        public static int EJobDeleted = 2200;
        public static int EJobNotFound = 2201;
        public static int EJobDisabled = 2202;

        // Client error
        public static int EPostcodeNotValid = 2300;
        public static int EPostcodeFromNotSupport = 2301;
        public static int EPostcodeToNotSupport = 2302;
        public static int EVoucherNotValid = 2303;
        public static int EVoucherExpired = 2304;
        public static int EVoucherMinimumRequired = 2305;
        public static int EVoucherRedemptionLimit = 2306;
    }
}