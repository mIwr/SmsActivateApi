using System;

namespace SmsActivate.API.Model
{
    /// <summary>
    /// SMS Activate activation status
    /// </summary>
    public enum SAActivationStatus: byte
    {
        /// <summary>
        /// Waiting SMS code
        /// </summary>
        WAIT_CODE,
        /// <summary>
        /// SMS code is received, but the activation isn't finished
        /// </summary>
        WAIT_RETRY,
        /// <summary>
        /// Waiting new SMS code
        /// </summary>
        WAIT_RESEND,
        /// <summary>
        /// Activation is finished (cancel)
        /// </summary>
        CANCEL,
        /// <summary>
        /// Activation is finished (success activation)
        /// </summary>
        OK
    }

    static class SAActivationStatusExt
    {
        public const byte NotifySmsSentApiCode = 1;

        public static SAActivationStatus[] Values
        {
            get
            {
                return Enum.GetValues<SAActivationStatus>();
            }
        }

        public static SAActivationStatus? From(string responseType)
        {
            foreach (var objType in Values)
            {
                if (responseType.CompareTo(objType.ResponseType()) == 0)
                {
                    return objType;
                }
            }
            return null;
        }

        public static SAActivationStatus? From(byte code)
        {
            foreach (var objType in Values)
            {
                if (code.CompareTo(objType.ApiCode()) == 0)
                {
                    return objType;
                }
            }
            return null;
        }

        public static string ResponseType(this SAActivationStatus type)
        {            
            switch (type)
            {
                case SAActivationStatus.WAIT_CODE: return "STATUS_WAIT_CODE";
                case SAActivationStatus.WAIT_RETRY: return "STATUS_WAIT_RETRY";
                case SAActivationStatus.WAIT_RESEND: return "STATUS_WAIT_RESEND";
                case SAActivationStatus.CANCEL: return "STATUS_CANCEL";
                case SAActivationStatus.OK: return "STATUS_OK";
            }
            return string.Empty;
        }

        public static byte ApiCode(this SAActivationStatus status)
        {
            switch (status)
            {
                case SAActivationStatus.WAIT_CODE: return 4;
                case SAActivationStatus.WAIT_RETRY: return 3;
                case SAActivationStatus.WAIT_RESEND: return 3;
                case SAActivationStatus.CANCEL: return 8;
                case SAActivationStatus.OK: return 6;
            }
            return 0;
        }

        public static string EngMsg(this SAActivationStatus status)
        {
            switch (status)
            {
                case SAActivationStatus.WAIT_CODE: return "Waiting sms.";
                case SAActivationStatus.WAIT_RETRY: return "Waiting for code clarification.";
                case SAActivationStatus.WAIT_RESEND: return "Waiting for re-sending SMS.";
                case SAActivationStatus.CANCEL: return "Activation canceled.";
                case SAActivationStatus.OK: return "Code received.";
                default: return "Unknown status activation.";
            }
        }

        public static string RusMsg(this SAActivationStatus status)
        {
            switch (status)
            {
                case SAActivationStatus.WAIT_CODE: return "Ожидание смс.";
                case SAActivationStatus.WAIT_RETRY: return "Ожидание уточнения кода.";
                case SAActivationStatus.WAIT_RESEND: return "Ожидание повторной отправки смс.";
                case SAActivationStatus.CANCEL: return "Активация отменена.";
                case SAActivationStatus.OK: return "Код получен.";
                default: return "Неизвестный статус активации.";
            }
        }

        public static string CombinedMessage(this SAActivationStatus status)
        {
            return status.EngMsg() + '|' + status.RusMsg();
        }
    }
}
