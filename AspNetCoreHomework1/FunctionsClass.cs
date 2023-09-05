using System.Globalization;



namespace AspNetCoreHomework1
{
    public static class FunctionsClass
    {
        public static float Duty(float price)
        {
            if (price > 200)
            {
                //15 процентов от суммы превышения
                return (float)((price - 200) * 0.15);
            }
            return 0; //нет пошлины
        }


        public static string GetDate(string language)
        {
            try
            {
                //выбираем формат введенного языка
                CultureInfo cultureInfoLanguage = new CultureInfo(language);
                return DateTime.Now.ToString(cultureInfoLanguage); //строка нужного формата
            }
            catch (Exception ex)
            {
                return "Ошибка. Такого языка нет!"; //если такого языка нет
            }
        }
    }
}
