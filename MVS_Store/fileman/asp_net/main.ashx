<%@ WebHandler Language="C#" Class="RoxyFilemanHandler" Debug="true" %>
/*
  RoxyFileman - web based file manager. Ready to use with CKEditor, TinyMCE. 
  Can be easily integrated with any other WYSIWYG editor or CMS.

  Copyright (C) 2013, RoxyFileman.com - Lyubomir Arsov. All rights reserved.
  For licensing, see LICENSE.txt or http://RoxyFileman.com/license

  This program is free software: you can redistribute it and/or modify
  it under the terms of the GNU General Public License as published by
  the Free Software Foundation, either version 3 of the License.

  This program is distributed in the hope that it will be useful,
  but WITHOUT ANY WARRANTY; without even the implied warranty of
  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
  GNU General Public License for more details.

  You should have received a copy of the GNU General Public License
  along with this program.  If not, see <http://www.gnu.org/licenses/>.

  Contact: Lyubomir Arsov, liubo (at) web-lobby.com
*/

using System;
using System.Reflection;
using System.Drawing;
using System.Collections;
using System.IO;
using System.Collections.Generic;
using System.Web;
using System.Text.RegularExpressions;
using System.Drawing.Imaging;
using System.IO.Compression;

public class RoxyFilemanHandler : IHttpHandler, System.Web.SessionState.IRequiresSessionState
{
	Dictionary<string, string> _settings = null;
	Dictionary<string, string> _lang = null;
	HttpResponse _r = null;
	HttpContext _context = null;
	string confFile = "../conf.json";
	public void ProcessRequest (HttpContext context) {
		_context = context;
		_r = context.Response;
		string action = "DIRLIST";
		try{
			if (_context.Request[key: "a"] != null)
				action = (string)_context.Request[key: "a"];

			VerifyAction(action: action);
			switch (action.ToUpper())
			{
				case "DIRLIST":
					ListDirTree(type: _context.Request[key: "type"]);
					break;
				case "FILESLIST":
					ListFiles(path: _context.Request[key: "d"], type: _context.Request[key: "type"]);
					break;
				case "COPYDIR":
					CopyDir(path: _context.Request[key: "d"], newPath: _context.Request[key: "n"]);
					break;
				case "COPYFILE":
					CopyFile(path: _context.Request[key: "f"], newPath: _context.Request[key: "n"]);
					break;
				case "CREATEDIR":
					CreateDir(path: _context.Request[key: "d"], name: _context.Request[key: "n"]);
					break;
				case "DELETEDIR":
					DeleteDir(path: _context.Request[key: "d"]);
					break;
				case "DELETEFILE":
					DeleteFile(path: _context.Request[key: "f"]);
					break;
				case "DOWNLOAD":
					DownloadFile(path: _context.Request[key: "f"]);
					break;
				case "DOWNLOADDIR":
					DownloadDir(path: _context.Request[key: "d"]);
					break;
				case "MOVEDIR":
					MoveDir(path: _context.Request[key: "d"], newPath: _context.Request[key: "n"]);
					break;
				case "MOVEFILE":
					MoveFile(path: _context.Request[key: "f"], newPath: _context.Request[key: "n"]);
					break;
				case "RENAMEDIR":
					RenameDir(path: _context.Request[key: "d"], name: _context.Request[key: "n"]);
					break;
				case "RENAMEFILE":
					RenameFile(path: _context.Request[key: "f"], name: _context.Request[key: "n"]);
					break;
				case "GENERATETHUMB":
					int w = 140, h = 0;
					int.TryParse(s: _context.Request[key: "width"].Replace(oldValue: "px", newValue: ""), result: out w);
					int.TryParse(s: _context.Request[key: "height"].Replace(oldValue: "px", newValue: ""), result: out h);
					ShowThumbnail(path: _context.Request[key: "f"], width: w, height: h);
					break;
				case "UPLOAD":
					Upload(path: _context.Request[key: "d"]);
					break;
				default:
					_r.Write(s: GetErrorRes(msg: "This action is not implemented."));
					break;
			}

		}
		catch(Exception ex){
			if (action == "UPLOAD" && !IsAjaxUpload())
			{
				_r.Write(s: "<script>");
				_r.Write(s: "parent.fileUploaded(" + GetErrorRes(msg: LangRes(name: "E_UploadNoFiles")) + ");");
				_r.Write(s: "</script>");
			}
			else{
				_r.Write(s: GetErrorRes(msg: ex.Message));
			}
		}

	}
	private string FixPath(string path)
	{
		if (!path.StartsWith(value: "~")){
			if (!path.StartsWith(value: "/"))
				path = "/" + path;
			path = "~" + path;
		}
		return _context.Server.MapPath(path: path);
	}
	private string GetLangFile(){
		string filename = "../lang/" + GetSetting(name: "LANG") + ".json";
		if (!File.Exists(path: _context.Server.MapPath(path: filename)))
			filename = "../lang/en.json";
		return filename;
	}
	protected string LangRes(string name)
	{
		string ret = name;
		if (_lang == null)
			_lang = ParseJSON(file: GetLangFile());
		if (_lang.ContainsKey(key: name))
			ret = _lang[key: name];

		return ret;
	}
	protected string GetFileType(string ext){
		string ret = "file";
		ext = ext.ToLower();
		if(ext == ".jpg" || ext == ".jpeg" || ext == ".png" || ext == ".gif")
			ret = "image";
		else if(ext == ".swf" || ext == ".flv")
			ret = "flash";
		return ret;
	}
	protected bool CanHandleFile(string filename)
	{
		bool ret = false;
		FileInfo file = new FileInfo(fileName: filename);
		string ext = file.Extension.Replace(oldValue: ".", newValue: "").ToLower();
		string setting = GetSetting(name: "FORBIDDEN_UPLOADS").Trim().ToLower();
		if (setting != "")
		{
			ArrayList tmp = new ArrayList();
			tmp.AddRange(c: Regex.Split(input: setting, pattern: "\\s+"));
			if (!tmp.Contains(item: ext))
				ret = true;
		}
		setting = GetSetting(name: "ALLOWED_UPLOADS").Trim().ToLower();
		if (setting != "")
		{
			ArrayList tmp = new ArrayList();
			tmp.AddRange(c: Regex.Split(input: setting, pattern: "\\s+"));
			if (!tmp.Contains(item: ext))
				ret = false;
		}

		return ret;
	}
	protected Dictionary<string, string> ParseJSON(string file){
		Dictionary<string, string> ret = new Dictionary<string,string>();
		string json = "";
		try{
			json = File.ReadAllText(path: _context.Server.MapPath(path: file), encoding: System.Text.Encoding.UTF8);
		}
		catch(Exception ex){}

		json = json.Trim();
		if(json != ""){
			if (json.StartsWith(value: "{"))
				json = json.Substring(startIndex: 1, length: json.Length - 2);
			json = json.Trim();
			json = json.Substring(startIndex: 1, length: json.Length - 2);
			string[] lines = Regex.Split(input: json, pattern: "\"\\s*,\\s*\"");
			foreach(string line in lines){
				string[] tmp = Regex.Split(input: line, pattern: "\"\\s*:\\s*\"");
				try{
					if (tmp[index0: 0] != "" && !ret.ContainsKey(key: tmp[index0: 0]))
					{
						ret.Add(key: tmp[index0: 0], value: tmp[index0: 1]);
					}
				}
				catch(Exception ex){}
			}
		}
		return ret;
	}
	protected string GetFilesRoot(){
		string ret = GetSetting(name: "FILES_ROOT");
		if (GetSetting(name: "SESSION_PATH_KEY") != "" && _context.Session[name: GetSetting(name: "SESSION_PATH_KEY")] != null)
			ret = (string)_context.Session[name: GetSetting(name: "SESSION_PATH_KEY")];

		if(ret == "")
			ret = _context.Server.MapPath(path: "../Uploads");
		else
			ret = FixPath(path: ret);
		return ret;
	}
	protected void LoadConf(){
		if(_settings == null)
			_settings = ParseJSON(file: confFile);
	}
	protected string GetSetting(string name){
		string ret = "";
		LoadConf();
		if(_settings.ContainsKey(key: name))
			ret = _settings[key: name];

		return ret;
	}
	protected void CheckPath(string path)
	{
		if (FixPath(path: path).IndexOf(value: GetFilesRoot()) != 0)
		{
			throw new Exception(message: "Access to " + path + " is denied");
		}
	}
	protected void VerifyAction(string action)
	{
		string setting = GetSetting(name: action);
		if (setting.IndexOf(value: "?") > -1)
			setting = setting.Substring(startIndex: 0, length: setting.IndexOf(value: "?"));
		if (!setting.StartsWith(value: "/"))
			setting = "/" + setting;
		setting = ".." + setting;

		if (_context.Server.MapPath(path: setting) != _context.Server.MapPath(path: _context.Request.Url.LocalPath))
			throw new Exception(message: LangRes(name: "E_ActionDisabled"));
	}
	protected string GetResultStr(string type, string msg)
	{
		return "{\"res\":\"" + type + "\",\"msg\":\"" + msg.Replace(oldValue: "\"",newValue: "\\\"") + "\"}";
	}
	protected string GetSuccessRes(string msg)
	{
		return GetResultStr(type: "ok", msg: msg);
	}
	protected string GetSuccessRes()
	{
		return GetSuccessRes(msg: "");
	}
	protected string GetErrorRes(string msg)
	{
		return GetResultStr(type: "error", msg: msg);
	}
	private void _copyDir(string path, string dest){
		if(!Directory.Exists(path: dest))
			Directory.CreateDirectory(path: dest);
		foreach(string f in  Directory.GetFiles(path: path)){
			FileInfo file = new FileInfo(fileName: f);
			if (!File.Exists(path: Path.Combine(path1: dest, path2: file.Name))){
				File.Copy(sourceFileName: f, destFileName: Path.Combine(path1: dest, path2: file.Name));
			}
		}
		foreach (string d in Directory.GetDirectories(path: path))
		{
			DirectoryInfo dir = new DirectoryInfo(path: d);
			_copyDir(path: d, dest: Path.Combine(path1: dest, path2: dir.Name));
		}
	}
	protected void CopyDir(string path, string newPath)
	{
		CheckPath(path: path);
		CheckPath(path: newPath);
		DirectoryInfo dir = new  DirectoryInfo(path: FixPath(path: path));
		DirectoryInfo newDir = new DirectoryInfo(path: FixPath(path: newPath + "/" + dir.Name));

		if (!dir.Exists)
		{
			throw new Exception(message: LangRes(name: "E_CopyDirInvalidPath"));
		}
		else if (newDir.Exists)
		{
			throw new Exception(message: LangRes(name: "E_DirAlreadyExists"));
		}
		else{
			_copyDir(path: dir.FullName, dest: newDir.FullName);
		}
		_r.Write(s: GetSuccessRes());
	}
	protected string MakeUniqueFilename(string dir, string filename){
		string ret = filename;
		int i = 0;
		while (File.Exists(path: Path.Combine(path1: dir, path2: ret)))
		{
			i++;
			ret = Path.GetFileNameWithoutExtension(path: filename) + " - Copy " + i.ToString() + Path.GetExtension(path: filename);
		}
		return ret;
	}
	protected void CopyFile(string path, string newPath)
	{
		CheckPath(path: path);
		FileInfo file = new FileInfo(fileName: FixPath(path: path));
		newPath = FixPath(path: newPath);
		if (!file.Exists)
			throw new Exception(message: LangRes(name: "E_CopyFileInvalisPath"));
		else{
			string newName = MakeUniqueFilename(dir: newPath, filename: file.Name);
			try{
				File.Copy(sourceFileName: file.FullName, destFileName: Path.Combine(path1: newPath, path2: newName));
				_r.Write(s: GetSuccessRes());
			}
			catch(Exception ex){
				throw new Exception(message: LangRes(name: "E_CopyFile"));
			}
		}
	}
	protected void CreateDir(string path, string name)
	{
		CheckPath(path: path);
		path = FixPath(path: path);
		if(!Directory.Exists(path: path))
			throw new Exception(message: LangRes(name: "E_CreateDirInvalidPath"));
		else{
			try
			{
				path = Path.Combine(path1: path, path2: name);
				if(!Directory.Exists(path: path))
					Directory.CreateDirectory(path: path);
				_r.Write(s: GetSuccessRes());
			}
			catch (Exception ex)
			{
				throw new Exception(message: LangRes(name: "E_CreateDirFailed"));
			}
		}
	}
	protected void DeleteDir(string path)
	{
		CheckPath(path: path);
		path = FixPath(path: path);
		if (!Directory.Exists(path: path))
			throw new Exception(message: LangRes(name: "E_DeleteDirInvalidPath"));
		else if (path == GetFilesRoot())
			throw new Exception(message: LangRes(name: "E_CannotDeleteRoot"));
		else if(Directory.GetDirectories(path: path).Length > 0 || Directory.GetFiles(path: path).Length > 0)
			throw new Exception(message: LangRes(name: "E_DeleteNonEmpty"));
		else
		{
			try
			{
				Directory.Delete(path: path);
				_r.Write(s: GetSuccessRes());
			}
			catch (Exception ex)
			{
				throw new Exception(message: LangRes(name: "E_CannotDeleteDir"));
			}
		}
	}
	protected void DeleteFile(string path)
	{
		CheckPath(path: path);
		path = FixPath(path: path);
		if (!File.Exists(path: path))
			throw new Exception(message: LangRes(name: "E_DeleteFileInvalidPath"));
		else
		{
			try
			{
				File.Delete(path: path);
				_r.Write(s: GetSuccessRes());
			}
			catch (Exception ex)
			{
				throw new Exception(message: LangRes(name: "E_DeletеFile"));
			}
		}
	}
	private List<string> GetFiles(string path, string type){
		List<string> ret = new List<string>();
		if(type == "#")
			type = "";
		string[] files = Directory.GetFiles(path: path);
		foreach(string f in files){
			if ((GetFileType(ext: new FileInfo(fileName: f).Extension) == type) || (type == ""))
				ret.Add(item: f);
		}
		return ret;
	}
	private ArrayList ListDirs(string path){
		string[] dirs = Directory.GetDirectories(path: path);
		ArrayList ret = new ArrayList();
		foreach(string dir in dirs){
			ret.Add(value: dir);
			ret.AddRange(c: ListDirs(path: dir));
		}
		return ret;
	}
	protected void ListDirTree(string type)
	{
		DirectoryInfo d = new DirectoryInfo(path: GetFilesRoot());
		if(!d.Exists)
			throw new Exception(message: "Invalid files root directory. Check your configuration.");

		ArrayList dirs = ListDirs(path: d.FullName);
		dirs.Insert(index: 0, value: d.FullName);

		string localPath = _context.Server.MapPath(path: "~/");
		_r.Write(s: "[");
		for(int i = 0; i <dirs.Count; i++){
			string dir = (string) dirs[index: i];
			_r.Write(s: "{\"p\":\"/" + dir.Replace(oldValue: localPath, newValue: "").Replace(oldValue: "\\", newValue: "/") + "\",\"f\":\"" + GetFiles(path: dir, type: type).Count.ToString() + "\",\"d\":\"" + Directory.GetDirectories(path: dir).Length.ToString() + "\"}");
			if(i < dirs.Count -1)
				_r.Write(s: ",");
		}
		_r.Write(s: "]");
	}
	protected double LinuxTimestamp(DateTime d){
		DateTime epoch = new DateTime(year: 1970, month: 1, day: 1, hour: 0, minute: 0, second: 0).ToLocalTime();
		TimeSpan timeSpan = (d.ToLocalTime() - epoch);

		return timeSpan.TotalSeconds;

	}
	protected void ListFiles(string path, string type)
	{
		CheckPath(path: path);
		string fullPath = FixPath(path: path);
		List<string> files = GetFiles(path: fullPath, type: type);
		_r.Write(s: "[");
		for(int i = 0; i < files.Count; i++){
			FileInfo f = new FileInfo(fileName: files[index: i]);
			int w = 0, h = 0;
			if (GetFileType(ext: f.Extension) == "image"){
				try{
					FileStream fs = new FileStream(path: f.FullName, mode: FileMode.Open, access: FileAccess.Read);
					Image img = Image.FromStream(stream: fs);
					w = img.Width;
					h = img.Height;
					fs.Close();
					fs.Dispose();
					img.Dispose();
				}
				catch(Exception ex){throw ex;}
			}
			_r.Write(s: "{");
			_r.Write(s: "\"p\":\""+path + "/" + f.Name+"\"");
			_r.Write(s: ",\"t\":\"" + Math.Ceiling(a: LinuxTimestamp(d: f.LastWriteTime)).ToString() + "\"");
			_r.Write(s: ",\"s\":\""+f.Length.ToString()+"\"");
			_r.Write(s: ",\"w\":\""+w.ToString()+"\"");
			_r.Write(s: ",\"h\":\""+h.ToString()+"\"");
			_r.Write(s: "}");
			if (i < files.Count - 1)
				_r.Write(s: ",");
		}
		_r.Write(s: "]");
	}
	public void DownloadDir(string path)
	{
		path = FixPath(path: path);
		if(!Directory.Exists(path: path))
			throw new Exception(message: LangRes(name: "E_CreateArchive"));
		string dirName = new FileInfo(fileName: path).Name;
		string tmpZip = _context.Server.MapPath(path: "../tmp/" + dirName + ".zip");
		if(File.Exists(path: tmpZip))
			File.Delete(path: tmpZip);
		ZipFile.CreateFromDirectory(sourceDirectoryName: path, destinationArchiveFileName: tmpZip,compressionLevel: CompressionLevel.Fastest, includeBaseDirectory: true);
		_r.Clear();
		_r.Headers.Add(name: "Content-Disposition", value: "attachment; filename=\"" + dirName + ".zip\"");
		_r.ContentType = "application/force-download";
		_r.TransmitFile(filename: tmpZip);
		_r.Flush();
		File.Delete(path: tmpZip);
		_r.End();
	}
	protected void DownloadFile(string path)
	{
		CheckPath(path: path);
		FileInfo file = new FileInfo(fileName: FixPath(path: path));
		if(file.Exists){
			_r.Clear();
			_r.Headers.Add(name: "Content-Disposition", value: "attachment; filename=\"" + file.Name + "\"");
			_r.ContentType = "application/force-download";
			_r.TransmitFile(filename: file.FullName);
			_r.Flush();
			_r.End();
		}
	}
	protected void MoveDir(string path, string newPath)
	{
		CheckPath(path: path);
		CheckPath(path: newPath);
		DirectoryInfo source = new DirectoryInfo(path: FixPath(path: path));
		DirectoryInfo dest = new DirectoryInfo(path: FixPath(path: Path.Combine(path1: newPath, path2: source.Name)));
		if(dest.FullName.IndexOf(value: source.FullName) == 0)
			throw new Exception(message: LangRes(name: "E_CannotMoveDirToChild"));
		else if (!source.Exists)
			throw new Exception(message: LangRes(name: "E_MoveDirInvalisPath"));
		else if (dest.Exists)
			throw new Exception(message: LangRes(name: "E_DirAlreadyExists"));
		else{
			try{
				source.MoveTo(destDirName: dest.FullName);
				_r.Write(s: GetSuccessRes());
			}
			catch(Exception ex){
				throw new Exception(message: LangRes(name: "E_MoveDir") + " \"" + path + "\"");
			}
		}

	}
	protected void MoveFile(string path, string newPath)
	{
		CheckPath(path: path);
		CheckPath(path: newPath);
		FileInfo source = new FileInfo(fileName: FixPath(path: path));
		FileInfo dest = new FileInfo(fileName: FixPath(path: newPath));

		if (!source.Exists)
			throw new Exception(message: LangRes(name: "E_MoveFileInvalisPath"));
		else if (dest.Exists)
			throw new Exception(message: LangRes(name: "E_MoveFileAlreadyExists"));
		else if (!CanHandleFile(filename: dest.Name))
			throw new Exception(message: LangRes(name: "E_FileExtensionForbidden"));
		else
		{
			try
			{
				source.MoveTo(destFileName: dest.FullName);
				_r.Write(s: GetSuccessRes());
			}
			catch (Exception ex)
			{
				throw new Exception(message: LangRes(name: "E_MoveFile") + " \"" + path + "\"");
			}
		}
	}
	protected void RenameDir(string path, string name)
	{
		CheckPath(path: path);
		DirectoryInfo source = new DirectoryInfo(path: FixPath(path: path));
		DirectoryInfo dest = new DirectoryInfo(path: Path.Combine(path1: source.Parent.FullName, path2: name));
		if(source.FullName == GetFilesRoot())
			throw new Exception(message: LangRes(name: "E_CannotRenameRoot"));
		else if (!source.Exists)
			throw new Exception(message: LangRes(name: "E_RenameDirInvalidPath"));
		else if (dest.Exists)
			throw new Exception(message: LangRes(name: "E_DirAlreadyExists"));
		else
		{
			try
			{
				source.MoveTo(destDirName: dest.FullName);
				_r.Write(s: GetSuccessRes());
			}
			catch (Exception ex)
			{
				throw new Exception(message: LangRes(name: "E_RenameDir") + " \"" + path + "\"");
			}
		}
	}
	protected void RenameFile(string path, string name)
	{
		CheckPath(path: path);
		FileInfo source = new FileInfo(fileName: FixPath(path: path));
		FileInfo dest = new FileInfo(fileName: Path.Combine(path1: source.Directory.FullName, path2: name));
		if (!source.Exists)
			throw new Exception(message: LangRes(name: "E_RenameFileInvalidPath"));
		else if (!CanHandleFile(filename: name))
			throw new Exception(message: LangRes(name: "E_FileExtensionForbidden"));
		else
		{
			try
			{
				source.MoveTo(destFileName: dest.FullName);
				_r.Write(s: GetSuccessRes());
			}
			catch (Exception ex)
			{
				throw new Exception(message: ex.Message + "; " + LangRes(name: "E_RenameFile") + " \"" + path + "\"");
			}
		}
	}
	public bool ThumbnailCallback()
	{
		return false;
	}

	protected void ShowThumbnail(string path, int width, int height)
	{
		CheckPath(path: path);
		FileStream fs = new FileStream(path: FixPath(path: path), mode: FileMode.Open, access: FileAccess.Read);
		Bitmap img = new Bitmap(original: Bitmap.FromStream(stream: fs));
		fs.Close();
		fs.Dispose();
		int cropWidth = img.Width, cropHeight = img.Height;
		int cropX = 0, cropY = 0;

		double imgRatio = (double)img.Width / (double)img.Height;

		if(height == 0)
			height = Convert.ToInt32(value: Math.Floor(d: (double)width / imgRatio));

		if (width > img.Width)
			width = img.Width;
		if (height > img.Height)
			height = img.Height;

		double cropRatio = (double)width / (double)height;
		cropWidth = Convert.ToInt32(value: Math.Floor(d: (double)img.Height * cropRatio));
		cropHeight = Convert.ToInt32(value: Math.Floor(d: (double)cropWidth / cropRatio));
		if (cropWidth > img.Width)
		{
			cropWidth = img.Width;
			cropHeight = Convert.ToInt32(value: Math.Floor(d: (double)cropWidth / cropRatio));
		}
		if (cropHeight > img.Height)
		{
			cropHeight = img.Height;
			cropWidth = Convert.ToInt32(value: Math.Floor(d: (double)cropHeight * cropRatio));
		}
		if(cropWidth < img.Width){
			cropX = Convert.ToInt32(value: Math.Floor(d: (double)(img.Width - cropWidth) / 2));
		}
		if(cropHeight < img.Height){
			cropY = Convert.ToInt32(value: Math.Floor(d: (double)(img.Height - cropHeight) / 2));
		}

		Rectangle area = new Rectangle(x: cropX, y: cropY, width: cropWidth, height: cropHeight);
		Bitmap cropImg = img.Clone(rect: area, format: System.Drawing.Imaging.PixelFormat.DontCare);
		img.Dispose();
		Image.GetThumbnailImageAbort imgCallback = new Image.GetThumbnailImageAbort(ThumbnailCallback);

		_r.AddHeader(name: "Content-Type", value: "image/png");
		cropImg.GetThumbnailImage(thumbWidth: width, thumbHeight: height, callback: imgCallback, callbackData: IntPtr.Zero).Save(stream: _r.OutputStream, format: ImageFormat.Png);
		_r.OutputStream.Close();
		cropImg.Dispose();
	}
	private ImageFormat GetImageFormat(string filename){
		ImageFormat ret = ImageFormat.Jpeg;
		switch(new FileInfo(fileName: filename).Extension.ToLower()){
			case ".png": ret = ImageFormat.Png; break;
			case ".gif": ret = ImageFormat.Gif; break;
		}
		return ret;
	}
	protected void ImageResize(string path, string dest, int width, int height)
	{
		FileStream fs = new FileStream(path: path, mode: FileMode.Open, access: FileAccess.Read);
		Image img = Image.FromStream(stream: fs);
		fs.Close();
		fs.Dispose();
		float ratio = (float)img.Width / (float)img.Height;
		if ((img.Width <= width && img.Height <= height) || (width == 0 && height == 0))
			return;

		int newWidth = width;
		int newHeight = Convert.ToInt16(value: Math.Floor(d: (float)newWidth / ratio));
		if ((height > 0 && newHeight > height) || (width == 0))
		{
			newHeight = height;
			newWidth = Convert.ToInt16(value: Math.Floor(d: (float)newHeight * ratio));
		}
		Bitmap newImg = new Bitmap(width: newWidth, height: newHeight);
		Graphics g = Graphics.FromImage(image: (Image)newImg);
		g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
		g.DrawImage(image: img, x: 0, y: 0, width: newWidth, height: newHeight);
		img.Dispose();
		g.Dispose();
		if(dest != ""){
			newImg.Save(filename: dest, format: GetImageFormat(filename: dest));
		}
		newImg.Dispose();
	}
	protected bool IsAjaxUpload()
	{
		return (_context.Request[key: "method"] != null && _context.Request[key: "method"].ToString() == "ajax");
	}
	protected void Upload(string path)
	{
		CheckPath(path: path);
		path = FixPath(path: path);
		string res = GetSuccessRes();
		bool hasErrors = false;
		try{
			for(int i = 0; i < HttpContext.Current.Request.Files.Count; i++){
				if (CanHandleFile(filename: HttpContext.Current.Request.Files[index: i].FileName))
				{
					FileInfo f = new FileInfo(fileName: HttpContext.Current.Request.Files[index: i].FileName);
					string filename = MakeUniqueFilename(dir: path, filename: f.Name);
					string dest = Path.Combine(path1: path, path2: filename);
					HttpContext.Current.Request.Files[index: i].SaveAs(filename: dest);
					if (GetFileType(ext: new FileInfo(fileName: filename).Extension) == "image")
					{
						int w = 0;
						int h = 0;
						int.TryParse(s: GetSetting(name: "MAX_IMAGE_WIDTH"), result: out w);
						int.TryParse(s: GetSetting(name: "MAX_IMAGE_HEIGHT"), result: out h);
						ImageResize(path: dest, dest: dest, width: w, height: h);
					}
				}
				else
				{
					hasErrors = true;
					res = GetSuccessRes(msg: LangRes(name: "E_UploadNotAll"));
				}
			}
		}
		catch(Exception ex){
			res = GetErrorRes(msg: ex.Message);
		}
		if (IsAjaxUpload())
		{
			if(hasErrors)
				res = GetErrorRes(msg: LangRes(name: "E_UploadNotAll"));
			_r.Write(s: res);
		}
		else
		{
			_r.Write(s: "<script>");
			_r.Write(s: "parent.fileUploaded(" + res + ");");
			_r.Write(s: "</script>");
		}
	}

	public bool IsReusable {
		get {
			return false;
		}
	}

}