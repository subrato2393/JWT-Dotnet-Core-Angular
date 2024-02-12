import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import {Router } from '@angular/router';
import { AuthService } from 'src/app/services/auth.service';

@Component({
  selector: 'app-signup',
  templateUrl: './signup.component.html',
  styleUrls: ['./signup.component.css']
})
export class SignupComponent implements OnInit {

  signupForm!:FormGroup
  isFormSubmitted=false;

  constructor(private fb:FormBuilder,private authService:AuthService, private router:Router) { }

  ngOnInit(): void {
   this.initializeForm();
  }
initializeForm(){
   this.signupForm=this.fb.group({
      firstname:['',Validators.required],
     lastname:[''],
      email:['',Validators.required],
      phoneno:['',Validators.required],
      password:['',Validators.required]
    })
}

  onSubmit(){
    this.isFormSubmitted = true;
    if(this.signupForm.valid){
      this.authService.signUp(this.signupForm.value).subscribe(
        (next)=>{
        console.log(next);
       // alert(next.message);
         // Clear the form after successful submission
         this.signupForm.reset();
         this.router.navigateByUrl('/login')
      },
      (error)=>{
        console.log(error);
        alert(error.error);
        this.signupForm.reset();
      }
      )
    }
    else{
 console.log("invalid");
    }
  }
  }

