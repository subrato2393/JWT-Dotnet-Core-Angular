import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { JwtHelperService } from '@auth0/angular-jwt';
import { TokenApiModel } from '../models/TokenApiModel';

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
  setRefreshToken(tokenValue:string){
   localStorage.setItem('refreshToken',tokenValue);
  }
  getRefreshToken():any{
    return localStorage.getItem('refreshToken');
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
   renewToken(tokenModel:TokenApiModel){
     return this.http.post<any>(`${this.baseUrl}User/refresh`,tokenModel);
   }
  getRoleFromToken():string{
    const token = this.getToken();
    const decodedToken = this.jwtHelper.decodeToken(token);
    return  decodedToken.role;
  }
}
