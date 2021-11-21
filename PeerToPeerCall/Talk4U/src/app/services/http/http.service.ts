import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Injectable } from '@angular/core';

@Injectable({
  providedIn: 'root'
})
export class HttpService {

  constructor(private _httpClient: HttpClient) { }

  public get(url: string, headers: HttpHeaders, withCredentials: boolean): Promise<any> {
    return this._httpClient.get(url, { headers, withCredentials }).toPromise();
  }

  public post(url: string, body: any, headers: HttpHeaders, withCredentials: boolean): Promise<any> {
    return this._httpClient.post(url, body, { headers, withCredentials }).toPromise();
  }
}
