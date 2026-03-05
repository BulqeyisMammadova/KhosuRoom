using System.Net;
using KhosuRoom.Business.Services.Abstractions;

namespace KhosuRoom.Business.Services.Implementations;

internal class EmailTemplateService : IEmailTemplateService
{
    public string BuildNotificationEmail(string title, string message, string? senderName = null, string? redirectUrl = null)
    {
        var t = WebUtility.HtmlEncode(title ?? string.Empty);
        var m = WebUtility.HtmlEncode(message ?? string.Empty).Replace("\n", "<br/>");
        var sender = string.IsNullOrWhiteSpace(senderName)
            ? string.Empty
            : $"<p style='margin:12px 0 0 0;font-size:14px;color:#555'><strong>Sent by:</strong> {WebUtility.HtmlEncode(senderName)}</p>";
        var cta = string.IsNullOrWhiteSpace(redirectUrl)
            ? string.Empty
            : $"<p style='text-align:center;margin:18px 0'><a href='{WebUtility.HtmlEncode(redirectUrl)}' style='display:inline-block;padding:10px 18px;background:#1a73e8;color:#fff;border-radius:6px;text-decoration:none;font-weight:600'>Open</a></p>";

        var html = $@"<!doctype html>
<html>
<head>
<meta charset='utf-8'/>
<title>{t}</title>
</head>
<body style='font-family:Arial,sans-serif;margin:0;padding:0;background:#f4f6f8'>
<table width='100%' cellpadding='0' cellspacing='0' role='presentation'>
<tr><td align='center'>
  <table width='600' cellpadding='0' cellspacing='0' role='presentation' style='background:#fff;margin:24px;border-radius:8px;overflow:hidden;box-shadow:0 2px 6px rgba(0,0,0,0.08)'>
    <tr>
      <td style='padding:18px 24px;background:#1a73e8;color:#fff'>
        <h2 style='margin:0;font-size:20px'>{t}</h2>
      </td>
    </tr>
    <tr>
      <td style='padding:20px 24px;color:#333;font-size:15px;line-height:1.45'>
        <div>{m}</div>
        {sender}
        {cta}
        <hr style='border:none;border-top:1px solid #eee;margin:16px 0'/>
        <p style='font-size:12px;color:#888;margin:0'>This message was sent from KhosuRoom. If you didn't expect this email, reply to the sender or contact support.</p>
      </td>
    </tr>
    <tr>
      <td style='padding:12px 24px;background:#fafafa;font-size:12px;color:#999;text-align:center'>© {DateTime.UtcNow.Year} KhosuRoom</td>
    </tr>
  </table>
</td></tr>
</table>
</body>
</html>";
        return html;
    }
}