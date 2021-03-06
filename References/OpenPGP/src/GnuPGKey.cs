/*
* Copyright (c) 2007-2008, Starksoft, LLC (http://www.starksoft.com)
* All rights reserved.
*
* Redistribution and use in source and binary forms, with or without
* modification, are permitted provided that the following conditions are met:
*     * Redistributions of source code must retain the above copyright
*       notice, this list of conditions and the following disclaimer.
*     * Redistributions in binary form must reproduce the above copyright
*       notice, this list of conditions and the following disclaimer in the
*       documentation and/or other materials provided with the distribution.
*     * Neither the name of Starsoft, LLC nor the
*       names of its contributors may be used to endorse or promote products
*       derived from this software without specific prior written permission.
*
* THIS SOFTWARE IS PROVIDED BY Starksoft, LLC ``AS IS'' AND ANY
* EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED
* WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE
* DISCLAIMED. IN NO EVENT SHALL Starksoft, LLC BE LIABLE FOR ANY
* DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES
* (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES;
* LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND
* ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT
* (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS
* SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
*/

using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace Starksoft.Cryptography.OpenPGP
{
  /// <summary>
  /// Class structure that proves a read-only view of the GnuPG keys. 
  /// </summary>
  public class GnuPGKey
  {
    private string _key;
    private DateTime _keyExpiration;
    private string _userId;
    private string _userName;
    private string _subKey;
    private DateTime _subKeyExpiration;
    private string _raw;

    /// <summary>
    /// GnuPGKey constructor.
    /// </summary>
    /// <param name="raw">Raw output stream text data containing key information.</param>
    public GnuPGKey(string raw)
    {
      _raw = raw;
      ParseRaw();
    }

    /// <summary>
    /// Key text information.
    /// </summary>
    public string Key
    {
      get { return _key; }
    }

    /// <summary>
    /// Key expiration date and time.
    /// </summary>
    public DateTime KeyExpiration
    {
      get { return _keyExpiration; }
    }

    /// <summary>
    /// Key user identification.
    /// </summary>
    public string UserId
    {
      get { return _userId; }
    }

    /// <summary>
    /// Key user name.
    /// </summary>
    public string UserName
    {
      get { return _userName; }
    }

    /// <summary>
    /// Sub-key information.
    /// </summary>
    public string SubKey
    {
      get { return _subKey; }
    }

    /// <summary>
    /// Sub-key expiration data and time.
    /// </summary>
    public DateTime SubKeyExpiration
    {
      get { return _subKeyExpiration; }
    }

    /// <summary>
    /// Raw output key text generated by GPG.EXE.
    /// </summary>
    public string Raw
    {
      get { return _raw; }
    }

    //sec   1024D/543C3595 2006-12-10
    //uid                  Benton Stark <benton@starksoft.com>
    //ssb   1024g/42A71AD8 2006-12-10
    //
    //pub   1024D/543C3595 2006-12-10
    //uid                  Benton Stark <benton@starksoft.com>
    //sub   1024g/42A71AD8 2006-12-10   
    private void ParseRaw()
    {
      char[] splitChar = { '\n', '\r' };
      string[] lines = _raw.Split(splitChar, StringSplitOptions.RemoveEmptyEntries);

      string[] pub = SplitSpaces(lines[0]);
      string uid = lines[1];

      if (lines.Length > 2)
      {
        string[] sub = SplitSpaces(lines[2]);
        _subKey = sub[1];
        _subKeyExpiration = DateTime.Parse(sub[2]);
      }
      else
      {
        _subKey = string.Empty;
        _subKeyExpiration = DateTime.MaxValue;
      }

      _key = pub[1];
      _keyExpiration = DateTime.Parse(pub[2]);

      ParseUid(uid);
    }

    private string[] SplitSpaces(string input)
    {
      char[] splitChar = { ' ' };
      return input.Split(splitChar, StringSplitOptions.RemoveEmptyEntries);
    }


    private void ParseUid(string uid)
    {
      Regex name = new Regex(@"(?<=uid).*(?=<)");
      Regex userId = new Regex(@"(?<=<).*(?=>)");

      _userName = name.Match(uid).ToString().Trim();
      _userId = userId.Match(uid).ToString();
    }

  }
}
