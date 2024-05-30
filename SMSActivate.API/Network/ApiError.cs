using System;

namespace SmsActivate.API.Network
{
    /// <summary>
    /// API error
    /// </summary>
    public partial class ApiError : Exception
    {
        /// <summary>
        /// Stock API response
        /// </summary>
        public string StockApiErrResponse { get; private set; }
        /// <summary>
        /// API error variant
        /// </summary>
        public ApiErrorEnum ErrType { get; private set; }        

        internal ApiError(string apiResponse, string msg, ApiErrorEnum? errType = null) : base(msg)
        {
            StockApiErrResponse = apiResponse;
            if (errType != null)
            {
                ErrType = errType.Value;
                return;
            }
            var parsed = ApiErrorExt.FromResponse(apiResponse) ?? ApiErrorEnum.BadResponse;
            ErrType = parsed;
        }
    }

    /// <summary>
    /// API error variant
    /// </summary>
    public enum ApiErrorEnum : byte
    {
        //Local
        /// <summary>
        /// No connection with network
        /// </summary>
        NoConnection,
        /// <summary>
        /// No received response
        /// </summary>
        NoResponse,
        /// <summary>
        /// Response has invalid data
        /// </summary>
        BadResponse,

        //API
        /// <summary>
        /// Account has banned (temporary or permonent)
        /// </summary>
        Banned,
        /// <summary>
        /// Invalid request action
        /// </summary>
        BadAction,
        /// <summary>
        /// Invalid service short name (ID)
        /// </summary>
        BadService,
        /// <summary>
        /// Invalid country ID
        /// </summary>
        BadCountry,
        /// <summary>
        /// Bad activation status
        /// </summary>
        BadStatus,
        /// <summary>
        /// Can't set 'Cancel' status
        /// </summary>
        CantCancel,
        /// <summary>
        /// Can't set 'OK' status - already set
        /// </summary>
        AlreadyFinish,
        /// <summary>
        /// Can't set 'Cancel' status - already set
        /// </summary>
        AlreadyCancel,
        /// <summary>
        /// Invalid max price
        /// </summary>
        WrongMaxPrice,
        /// <summary>
        /// API token is empty
        /// </summary>
        NoToken,
        /// <summary>
        /// Bad API token
        /// </summary>
        BadToken,
        /// <summary>
        /// Insufficient funds for operation
        /// </summary>
        NoBalance,
        /// <summary>
        /// Insufficient funds for forward operation
        /// </summary>
        NoBalanceForward,
        /// <summary>
        /// No numbers for operation (activation or rent)
        /// </summary>
        NoNumbers,
        /// <summary>
        /// Account has no activations
        /// </summary>
        NoActivations,
        /// <summary>
        /// API SQL error
        /// </summary>
        ErrorSQL,
        /// <summary>
        /// Invalid activation ID
        /// </summary>
        WrongActivationID
    }
    /*
     'ACCESS_ACTIVATION' => 'Сервис успешно активирован',
        'ACCESS_CANCEL'     => 'активация отменена',
        'ACCESS_READY'      => 'Ожидание нового смс',
        'ACCESS_RETRY_GET'  => 'Готовность номера подтверждена',
        'ACCOUNT_INACTIVE'  => 'Свободных номеров нет',
        'ALREADY_FINISH'    => 'Аренда уже завершена',
        'ALREADY_CANCEL'    => 'Аренда уже отменена',
        'BAD_ACTION'        => 'Некорректное действие (параметр action)',
        'BAD_SERVICE'       => 'Некорректное наименование сервиса (параметр service)',
        'NO_KEY'            => 'Не указан API ключ доступа или ключ пустой'
        'BAD_KEY'           => 'Неверный API ключ доступа',
        'BAD_STATUS'        => 'Попытка установить несуществующий статус',
        'BANNED'            => 'Аккаунт заблокирован',
        'CANT_CANCEL'       => 'Невозможно отменить аренду (прошло более 20 мин.)',
        'ERROR_SQL'         => 'Один из параметров имеет недопустимое значение',
        'NO_NUMBERS'        => 'Нет свободных номеров для приёма смс от текущего сервиса',
        'NO_BALANCE'        => 'Закончился баланс',
        'NO_YULA_MAIL'      => 'Необходимо иметь на счету более 500 рублей для покупки сервисов холдинга Mail.ru и Mamba',
        'NO_CONNECTION'     => 'Нет соединения с серверами sms-activate',
        'NO_ID_RENT'        => 'Не указан id аренды',
        'NO_ACTIVATION'     => 'Указанного id активации не существует',
        'STATUS_CANCEL'     => 'Активация/аренда отменена',
        'STATUS_FINISH'     => 'Аренда оплачена и завершена',
        'STATUS_WAIT_CODE'  => 'Ожидание первой смс',
        'STATUS_WAIT_RETRY' => 'ожидание уточнения кода',
        'SQL_ERROR'         => 'Один из параметров имеет недопустимое значение',
        'INVALID_PHONE'     => 'Номер арендован не вами (неправильный id аренды)',
        'INCORECT_STATUS'   => 'Отсутствует или неправильно указан статус',
        'WRONG_SERVICE'     => 'Сервис не поддерживает переадресацию',
        'WRONG_SECURITY'    => 'Ошибка при попытке передать ID активации без переадресации, или же завершенной/не активной активации'
     */

    /// <summary>
    /// API error variants extension
    /// </summary>
    public static class ApiErrorExt
    {
        /// <summary>
        /// 'Ban' response key
        /// </summary>
        public const string BannedKey = "BANNED";
        /// <summary>
        /// Invalid request action parameter
        /// </summary>
        public const string BadActionKey = "BAD_ACTION";
        /// <summary>
        /// Invalid request service parameter
        /// </summary>
        public const string BadServiceKey = "BAD_SERVICE";
        /// <summary>
        /// Invalid request activation or rent parameter
        /// </summary>
        public const string BadStatusKey = "BAD_STATUS";
        /// <summary>
        /// Can't cancel rent or activation
        /// </summary>
        public const string CantCancelKey = "CANT_CANCEL";
        /// <summary>
        /// The rent or activation has already been finished
        /// </summary>
        public const string AlreadyFinishKey = "ALREADY_FINISH";
        /// <summary>
        /// The rent or activation has already been cancelled
        /// </summary>
        public const string AlreadyCancelKey = "ALREADY_CANCEL";
        /// <summary>
        /// Invalid max price response
        /// </summary>
        public const string WrongMaxPriceKey = "WRONG_MAX_PRICE";
        /// <summary>
        /// No provided any API token response
        /// </summary>
        public const string NoTokenKey = "NO_KEY";
        /// <summary>
        /// Invaliad API token response
        /// </summary>
        public const string BadTokenKey = "BAD_KEY";
        /// <summary>
        /// 'Insufficient funds for the operation' response key
        /// </summary>
        public const string NoBalanceKey = "NO_BALANCE";
        /// <summary>
        /// 'Insufficient funds for the forward operation' response key
        /// </summary>
        public const string NoBalanceForwardKey = "NO_BALANCE_FORWARD";
        /// <summary>
        /// No numbers for operation (activation or rent)
        /// </summary>
        public const string NoNumbersKey = "NO_NUMBERS";
        /// <summary>
        /// Account has no activations
        /// </summary>
        public const string NoActivationsKey = "NO_ACTIVATIONS";
        /// <summary>
        /// API SQL error
        /// </summary>
        public const string ErrorSqlKey = "ERROR_SQL";
        /// <summary>
        /// API SQL error
        /// </summary>
        public const string ErrorSqlReverseKey = "SQL_ERROR";
        /// <summary>
        /// Invalid activation ID
        /// </summary>
        public const string WrongActivationIdKey = "WRONG_ACTIVATION_ID";

        /// <summary>
        /// All API error variants
        /// </summary>
        public static ApiErrorEnum[] Values
        {
            get
            {
                return Enum.GetValues<ApiErrorEnum>();
            }
        }

        /// <summary>
        /// Try parse API error variant from response
        /// </summary>
        /// <param name="response">Stock API response</param>
        /// <returns>Optional API error variant</returns>
        public static ApiErrorEnum? FromResponse(string response)
        {
            var uppercased = response.ToUpper();
            if (uppercased.Contains(ErrorSqlKey) || uppercased.Contains(ErrorSqlReverseKey))
            {
                return ApiErrorEnum.ErrorSQL;
            }
            if (uppercased.Contains(BannedKey))
            {
                return ApiErrorEnum.Banned;
            }
            if (uppercased.Contains(BadActionKey))
            {
                return ApiErrorEnum.BadAction;
            }
            if (uppercased.Contains(BadServiceKey))
            {
                return ApiErrorEnum.BadService;
            }
            if (uppercased.Contains(BadStatusKey))
            {
                return ApiErrorEnum.BadStatus;
            }
            if (uppercased.Contains(CantCancelKey))
            {
                return ApiErrorEnum.CantCancel;
            }
            if (uppercased.Contains(AlreadyFinishKey))
            {
                return ApiErrorEnum.AlreadyFinish;
            }
            if (uppercased.Contains(AlreadyCancelKey))
            {
                return ApiErrorEnum.AlreadyCancel;
            }
            if (uppercased.Contains(WrongMaxPriceKey))
            {
                return ApiErrorEnum.WrongMaxPrice;
            }
            if (uppercased.Contains(NoTokenKey))
            {
                return ApiErrorEnum.NoToken;
            }
            if (uppercased.Contains(BadTokenKey))
            {
                return ApiErrorEnum.BadToken;
            }
            if (uppercased.Contains(NoBalanceKey))
            {
                return ApiErrorEnum.NoBalance;
            }
            if (uppercased.Contains(NoBalanceForwardKey))
            {
                return ApiErrorEnum.NoBalanceForward;
            }
            if (uppercased.Contains(NoNumbersKey))
            {
                return ApiErrorEnum.NoNumbers;
            }
            if (uppercased.Contains(NoActivationsKey))
            {
                return ApiErrorEnum.NoActivations;
            }
            if (uppercased.Contains(WrongActivationIdKey))
            {
                return ApiErrorEnum.WrongActivationID;
            }
            return null;
        }

        /// <summary>
        /// API error english message
        /// </summary>
        /// <param name="error"></param>
        /// <returns></returns>
        public static string Message(this ApiErrorEnum error)
        {
            switch (error)
            {
                case ApiErrorEnum.NoConnection: return "No network connection";
                case ApiErrorEnum.NoResponse: return "Response is null";
                case ApiErrorEnum.BadResponse: return "Bad response data";
                case ApiErrorEnum.BadAction: return "Invalid action name";
                case ApiErrorEnum.BadCountry: return "Invalid country ID";
                case ApiErrorEnum.BadService: return "Invalid serivce ID (short name)";
                case ApiErrorEnum.BadStatus: return "Invalid status ID set attempt";
                case ApiErrorEnum.CantCancel: return "It is impossible to cancel the rent(more than 20 min.)";
                case ApiErrorEnum.AlreadyFinish: return "Rent or activation has already been finished";
                case ApiErrorEnum.AlreadyCancel: return "Rent or activation has already been cancelled";
                case ApiErrorEnum.WrongMaxPrice: return "Max price is lower than minimal";
                case ApiErrorEnum.BadToken: return "Invalid API token";
                case ApiErrorEnum.NoBalance: return "Insufficient funds for operation";
                case ApiErrorEnum.NoBalanceForward: return "Insufficient funds for forward operation";
                case ApiErrorEnum.NoActivations: return "No available numbers";
                case ApiErrorEnum.Banned: return "Account is banned";
                case ApiErrorEnum.ErrorSQL: return "Parameter invalid value";
                case ApiErrorEnum.WrongActivationID: return "Invalid activation ID";
            }
            return string.Empty;
        }

        /// <summary>
        /// Transforms API error variant as API error instance
        /// </summary>
        /// <param name="error">API error variant</param>
        /// <param name="apiResponse">Stock API response</param>
        /// <param name="message">Exception message override</param>
        /// <returns>API error instance</returns>
        public static ApiError AsException(this ApiErrorEnum error, string apiResponse, string? message = null)
        {
            return new ApiError(apiResponse, message ?? error.Message(), error);
        }
    }
}
