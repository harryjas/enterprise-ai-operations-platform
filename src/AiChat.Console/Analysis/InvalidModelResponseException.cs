namespace AiChat.Analysis;
public class InvalidModelResponseException(string message, Exception? inner = null)
    : Exception(message, inner);