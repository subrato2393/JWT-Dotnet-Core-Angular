import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { ToastrService } from 'ngx-toastr';
import { AuthService } from 'src/app/services/auth.service';

@Component({
  selector: 'app-dashboard',
  templateUrl: './dashboard.component.html',
  styleUrls: ['./dashboard.component.css']
})
export class DashboardComponent implements OnInit {
  users!:any[];
  username!: string;
  role!: string;

  constructor(private authService:AuthService,private router:Router,private toastService:ToastrService) { }
  ngOnInit(): void {
    this.username =  this.authService.getUserNameFromToken();
    this.role = this.authService.getRoleFromToken();
    this.getAllUserList();
  }

  onLogout(){
      this.authService.resetToken();
      this.router.navigateByUrl('/login');
      this.toastService.success('Logout successfully');
  }
   getAllUserList(){
    console.log('role')
    console.log(this.role)
    console.log('Name')
    console.log(this.username)
    this.authService.getAllUsers().subscribe(
      (res)=>
        {
           this.users=res
           console.log(this.users);
        },
      (err)=>
        {
           console.log('Error fetching data',err);
        }
      )
   }
 
}
