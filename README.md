# UserAgentDetector
На базе https://github.com/totpero/DeviceDetector.NET разработано API с использование .NET Core подобное https://useragent.app/ которое будет по указанному UserAgent-у выводить информацию об устройстве пользователя в JSON-формате.  Если UserAgent не указан в GET-параметрах, то необходимо использовать текущий из HTTP-заголовков.
