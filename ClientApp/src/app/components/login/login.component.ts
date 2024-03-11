import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { Router } from '@angular/router';
import { AuthService } from 'src/app/services/auth.service';
import { ToastrService } from 'ngx-toastr';

@Component({
  selector: 'app-login',
  templateUrl: './login.component.html',
  styleUrls: ['./login.component.css']
})
export class LoginComponent implements OnInit {

  loginForm!:FormGroup;   
  isFormSubmitted:boolean = false;

  constructor(private fb:FormBuilder,private router:Router,private authService:AuthService, private toastService:ToastrService) { }

  ngOnInit(): void {
    this.loginForm =this.fb.group({
      firstname:['',Validators.required],
       password:['',Validators.required]
    })
  }

  onSubmit(){
    this.isFormSubmitted = true;
    if(this.loginForm.valid){
     this.authService.login(this.loginForm.value).subscribe(
      (next)=>
      {
        this.authService.setToken(next.token)
        console.log("token");
        console.log(next.token);
        //const nextJson = JSON.stringify(next);
        this.toastService.success("Login Success",'Success',{
         timeOut:1000
        })
      //  alert(next.message)
        this.router.navigateByUrl('/dashboard')
        this.loginForm.reset();
      },
      (error)=>
      {
        this.toastService.error("Failed")
      //  alert(error.error);
        this.loginForm.reset();
      })
    }
    else{
 console.log("invalid");
    }
  }
}
