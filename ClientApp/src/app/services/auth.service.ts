import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { JwtHelperService } from '@auth0/angular-jwt';

@Injectable({
  providedIn: 'root'
})
export class AuthService {

  private jwtHelper = new JwtHelperService();
  private baseUrl="https://localhost:7189/api/";
  constructor(private http:HttpClient) { }

  getAllUsers(){
    return this.http.get<any>(`${this.baseUrl}User`)
  }
  login(userObj:any){
   return this.http.post<any>(`${this.baseUrl}User/login`,userObj);
  }
  signUp(userObj:any){
    return this.http.post<any>(`${this.baseUrl}User/signup`,userObj)
  }
  getToken():any{
    return localStorage.getItem('token');
  }
  setToken(tokenValue:string){
   localStorage.setItem('token',tokenValue);
  }
  isLoggedIn():boolean{
   return !!localStorage.getItem('token');
  }
  resetToken(){
    localStorage.clear();
  }

  getUserNameFromToken():string{
     const token= this.getToken();
     const decodedToken = this.jwtHelper.decodeToken(token)
     return decodedToken.name;
  }

  getRoleFromToken():string{
    const token = this.getToken();
    const decodedToken = this.jwtHelper.decodeToken(token);
    return  decodedToken.role;
  }
}
