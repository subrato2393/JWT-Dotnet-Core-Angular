import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';

@Injectable({
  providedIn: 'root'
})
export class AuthService {

  private baseUrl="https://localhost:7189/api/";
  constructor(private http:HttpClient) { }

  login(userObj:any){
   return this.http.post<any>(`${this.baseUrl}User/login`,userObj);
  }
  signUp(userObj:any){
    return this.http.post<any>(`${this.baseUrl}User/signup`,userObj)
  }
}
