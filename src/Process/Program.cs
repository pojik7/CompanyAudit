using CompaniesAudit.Conteiner;
using System;


namespace Process
{
	class Program
	{
		static void Main(string[] args)
		{
			try
			{
				var task = (new CompaniesAuditProcess()).RunAsync(args);
				task.Wait();
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex);
				Console.ReadLine();
			}
		}

	}
}
