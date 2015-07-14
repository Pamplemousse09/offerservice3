using Kikai.Logging.Utils;
using Kikai.Logging.Utils.IUtils;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Kikai.Logging.DTO
{
    [DataContract(Namespace = "", Name = "Item")]
    public class ErrorObject
    {
        public int Id { get; set; }
        public string ErrorKey { get; set; }
        [DataMember(Order = 1)]
        public int Code { get; set; }
        [DataMember(Order = 2)]
        public string Message { get; set; }

        public ErrorObject() { }

        /// <summary>
        /// Constructor that will create the ErrorObject based on the Error Id
        /// </summary>
        /// <param name="ErrorId"></param>
        public ErrorObject(int ErrorId, Dictionary<string, string> parameters = null)
        {
            IErrorUtil ErrorUtil = new ErrorUtil();
            ErrorObject ErrorObject = ErrorUtil.GetError(ErrorId);
            this.Id = ErrorObject.Id;
            this.ErrorKey = ErrorObject.ErrorKey;
            this.Code = ErrorObject.Code;
            //If there is no parameters then put the error message as is
            if (parameters == null)
                this.Message = ErrorObject.Message;
            //If parameters is not null then replace each variable in the error message with it's appropriate parameter
            else
            {
                var message = ErrorObject.Message;
                foreach (var parameter in parameters)
                {
                    if (ErrorObject.Message.Contains(parameter.Key))
                        message = message.Replace(parameter.Key, parameter.Value);
                }
                this.Message = message;
            }
        }

        /// <summary>
        /// Constructor that will create the ErrorObject based on passed parameters
        /// </summary>
        /// <param name="ErrorKey"></param>
        /// <param name="Code"></param>
        /// <param name="Message"></param>
        public ErrorObject(string id, string ErrorKey, string Code, string Message)
        {
            this.Id = Convert.ToInt32(id);
            this.ErrorKey = ErrorKey;
            this.Code = Convert.ToInt32(Code);
            this.Message = Message;
        }
    }
}
