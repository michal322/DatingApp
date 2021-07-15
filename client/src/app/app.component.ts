import { HttpClient } from '@angular/common/http';
import { Component, OnInit } from '@angular/core';
import { User } from './_models/user';
import { AccountService } from './_Services/account.service';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.css']
})
export class AppComponent implements OnInit {
  title = 'Dating App';
  users:any;
  constructor(private accountSrvice:AccountService){}

  ngOnInit() {
   
   this.setCurrentUser();
   
  }
  setCurrentUser(){
    const user:User=JSON.parse(localStorage.getItem('user') as any);
    this.accountSrvice.setCurrentuser(user);
  }

}
