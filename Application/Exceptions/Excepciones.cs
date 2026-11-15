namespace Application.Exceptions;

public class NotFoundException(string mensaje) : Exception(mensaje);
public class ConflictException(string mensaje) : Exception(mensaje);
public class ValidacionException(string mensaje) : Exception(mensaje);