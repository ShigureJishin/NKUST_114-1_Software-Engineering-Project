using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Text.Json;
using System.Text;
using System.IO;
using System.Threading.Tasks;
using System;
using Microsoft.AspNetCore.Http;

namespace WebApplication1.Controllers
{
    public class UploadController : Controller
    {
        [HttpGet]
        public IActionResult SingleForm()
        {
            // return the combined view
            return View("SingleAndFile");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SingleToFile(SingleRecordModel model)
        {
            if (!ModelState.IsValid)
            {
                return View("SingleAndFile", model);
            }

            // build JSON array expected by ConsoleApp
            var item = new
            {
                Date = model.Date ?? string.Empty,
                Code = model.Code ?? string.Empty,
                Name = model.Name ?? string.Empty,
                TradeVolume = model.TradeVolume ?? string.Empty,
                TradeValue = model.TradeValue ?? string.Empty,
                OpeningPrice = model.OpeningPrice ?? string.Empty,
                HighestPrice = model.HighestPrice ?? string.Empty,
                LowestPrice = model.LowestPrice ?? string.Empty,
                ClosingPrice = model.ClosingPrice ?? string.Empty,
                Change = model.Change ?? string.Empty,
                Transaction = model.Transaction ?? string.Empty
            };

            var list = new[] { item };
            var json = JsonSerializer.Serialize(list);

            // write to temp file
            var tempFile = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString() + ".json");
            await System.IO.File.WriteAllTextAsync(tempFile, json, Encoding.UTF8);

            string result;
            var consoleExe = Path.Combine(Directory.GetCurrentDirectory(), "..", "ConsoleApp1", "bin", "Debug", "net9.0", "ConsoleApp1.exe");

            if (!System.IO.File.Exists(consoleExe))
            {
                result = $"Console exe not found: {consoleExe}";
                // cleanup temp
                try { System.IO.File.Delete(tempFile); } catch { }
                ViewBag.Result = result;
                return View("SingleAndFile", model);
            }

            try
            {
                var psi = new ProcessStartInfo
                {
                    FileName = consoleExe,
                    Arguments = '"' + tempFile + '"',
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    UseShellExecute = false,
                    CreateNoWindow = true,
                };

               using (var proc = Process.Start(psi))
                {
                    var output = await proc.StandardOutput.ReadToEndAsync();
                    var err = await proc.StandardError.ReadToEndAsync();
                    await proc.WaitForExitAsync();

                    // try to find RESULT line
                    result = null;
                    foreach (var line in output.Split('\n'))
                    {
                        if (line.StartsWith("RESULT:"))
                        {
                            result = line.Substring("RESULT:".Length).Trim();
                            break;
                        }
                    }

                    if (result == null)
                    {
                        result = string.IsNullOrWhiteSpace(output) ? err : output + (string.IsNullOrWhiteSpace(err) ? "" : "\nERR:\n" + err);
                    }
                }
            }
            catch (Exception ex)
            {
                result = "ERROR: " + ex.Message;
            }
            finally
            {
                try { System.IO.File.Delete(tempFile); } catch { }
            }

            ViewBag.Result = result;
            return View("SingleAndFile", model);
        }

        // New: file upload form
        [HttpGet]
        public IActionResult FileForm()
        {
            // return the combined view
            return View("SingleAndFile");
        }

        // New: upload file and pass saved file path to ConsoleApp
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> FileToConsole(IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                ModelState.AddModelError("file", "請上傳 JSON 檔案");
                return View("SingleAndFile");
            }

            // save uploaded file to temp path
            var tempFile = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString() + ".json");
            try
            {
                using (var fs = System.IO.File.Create(tempFile))
                {
                    await file.CopyToAsync(fs);
                }
            }
            catch (Exception ex)
            {
                ViewBag.Result = "ERROR: unable to save uploaded file: " + ex.Message;
                return View("SingleAndFile");
            }

            string result;
            var consoleExe = Path.Combine(Directory.GetCurrentDirectory(), "..", "ConsoleApp1", "bin", "Debug", "net9.0", "ConsoleApp1.exe");
            if (!System.IO.File.Exists(consoleExe))
            {
                result = $"Console exe not found: {consoleExe}";
                try { System.IO.File.Delete(tempFile); } catch { }
                ViewBag.Result = result;
                return View("SingleAndFile");
            }

            try
            {
                var psi = new ProcessStartInfo
                {
                    FileName = consoleExe,
                    Arguments = '"' + tempFile + '"',
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    UseShellExecute = false,
                    CreateNoWindow = true,
                };

                using (var proc = Process.Start(psi))
                {
                    var output = await proc.StandardOutput.ReadToEndAsync();
                    var err = await proc.StandardError.ReadToEndAsync();
                    await proc.WaitForExitAsync();

                    result = null;
                    foreach (var line in output.Split('\n'))
                    {
                        if (line.StartsWith("RESULT:"))
                        {
                            result = line.Substring("RESULT:".Length).Trim();
                            break;
                        }
                    }

                    if (result == null)
                    {
                        result = string.IsNullOrWhiteSpace(output) ? err : output + (string.IsNullOrWhiteSpace(err) ? "" : "\nERR:\n" + err);
                    }
                }
            }
            catch (Exception ex)
            {
                result = "ERROR: " + ex.Message;
            }
            finally
            {
                try { System.IO.File.Delete(tempFile); } catch { }
            }

            ViewBag.Result = result;
            return View("SingleAndFile");
        }
    }

    public class SingleRecordModel
    {
        // Date in yyyyMMdd
        public string Date { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public string TradeVolume { get; set; }
        public string TradeValue { get; set; }
        public string OpeningPrice { get; set; }
        public string HighestPrice { get; set; }
        public string LowestPrice { get; set; }
        public string ClosingPrice { get; set; }
        public string Change { get; set; }
        public string Transaction { get; set; }
    }
}
