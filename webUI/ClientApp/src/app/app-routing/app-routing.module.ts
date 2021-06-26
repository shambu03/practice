import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { CommonModule } from '@angular/common';
import { MastercardsFgComponent } from 'src/app/mastercards-fg/mastercards-fg.component'
import { MastercardPdsComponent } from 'src/app/mastercard-pds/mastercard-pds.component'
import { HomeComponent } from 'src/app/home/home.component'


const routes: Routes = [
  { path: '', component: HomeComponent },
  { path: 'mastercardfg', component: MastercardsFgComponent, },
  { path: 'mastercardpds', component: MastercardPdsComponent, },
];


//@NgModule({
//  declarations: [],
//  imports: [
//    CommonModule
//  ]
//})
@NgModule({
  imports: [
    RouterModule.forRoot(routes)
  ],
  exports: [
    RouterModule
  ],
  declarations: []
})
export class AppRoutingModule { }
