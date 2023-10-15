using System.Diagnostics;

namespace hm8
{
    public class MemoryChecker : IMemoryUsedChecker
    {
        private readonly Process currentProcess;

        public MemoryChecker()
        {
            //получаем процесс
            currentProcess = Process.GetCurrentProcess();
        }
        public MemoryChecker(Process currentProcess)
        {
            this.currentProcess = currentProcess;
        }
        public string CheckMemoryApp()
        {
            //обновили данные
            currentProcess.Refresh();
            //вернули string
            return $"Используется памяти приложением: {currentProcess.WorkingSet64 / 1024}";
        }
        public Process GetProcess() { return currentProcess; }
    }
}
