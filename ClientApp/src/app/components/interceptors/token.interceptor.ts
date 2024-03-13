import { Injectable } from '@angular/core';
import {
  HttpRequest,
  HttpHandler,
  HttpEvent,
  HttpInterceptor
} from '@angular/common/http';
import { Observable } from 'rxjs';
import { AuthService } from 'src/app/services/auth.service';

@Injectable()
export class TokenInterceptor implements HttpInterceptor {

  constructor(private authService:AuthService) {}

  intercept(request: HttpRequest<unknown>, next: HttpHandler): Observable<HttpEvent<unknown>> {
  //  const myToken= localStorage.getItem('token');
  const myToken= this.authService.getToken();
    console.log("--"+myToken);
    request = request.clone({
      setHeaders: {
        Authorization: `Bearer ${myToken}`
      }
    });

    return next.handle(request);
  }
}
