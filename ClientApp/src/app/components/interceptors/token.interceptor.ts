import { Injectable } from '@angular/core';
import {
  HttpRequest,
  HttpHandler,
  HttpEvent,
  HttpInterceptor,
  HttpErrorResponse
} from '@angular/common/http';
import { Observable, throwError } from 'rxjs';
import { AuthService } from 'src/app/services/auth.service';
import {  Router } from '@angular/router';
import { catchError, switchMap } from 'rxjs/operators';
import { ToastrService } from 'ngx-toastr';
import { TokenApiModel } from 'src/app/models/TokenApiModel';

@Injectable()
export class TokenInterceptor implements HttpInterceptor {

  constructor(private authService:AuthService,private router:Router, private toaster:ToastrService) {}

  intercept(request: HttpRequest<unknown>, next: HttpHandler): Observable<HttpEvent<unknown>> {
  //  const myToken= localStorage.getItem('token');
  const myToken= this.authService.getToken();
    console.log("--"+myToken);
    request = request.clone({
      setHeaders: {
        Authorization: `Bearer ${myToken}`
      }
    })

    return next.handle(request).pipe(
       catchError((err:any)=>{
         if(err instanceof HttpErrorResponse){
          if(err.status === 401){
            return this.handleUnaAuthorizedError(request,next);
           // alert("token expired");
           // this.toaster.warning('Your token is expired, Please login again')
           // this.router.navigate(['login'])
          }
         }
         return throwError(()=>{
           new Error('some error occured');
         })
       })
    );

 
  }
  handleUnaAuthorizedError(request: HttpRequest<unknown>, next: HttpHandler){
      let tokenApiModel = new TokenApiModel();
      console.log('rrrrrrrrrrrrrrr');
      tokenApiModel.accessToken = this.authService.getToken();
      tokenApiModel.refreshToken = this.authService.getRefreshToken();
      return this.authService.renewToken(tokenApiModel)
      .pipe(
        switchMap((data:TokenApiModel)=>{
          console.log('rrrrrrrrrrrrrrr');
           this.authService.setToken(data.accessToken);
           this.authService.setRefreshToken(data.refreshToken);
           request = request.clone({setHeaders:{
            Authorization:`Bearer${data.accessToken}`
           }})
           return next.handle(request);
        }),
         catchError((error:any)=>{
           return throwError(()=>{
             this.toaster.warning('Your token is expired, Please login again');
             this.router.navigate(['login']);
             return throwError(error);
           })
         })
      )
  }
}
