#!/usr/bin/perl
#
# PESectionExtractor v0.2, written by Adam Blaszczyk @ Hexacorn.com, 2012-09
# Modified by ANDRE SUSANTO 2016 to comply with AHMDS Static Analyzer Engine
#

use strict;
use warnings;

$| = 1;

my $extract_strings=0;

my $arg = shift;
if (!defined($arg))
{
  print "AHMDS_ERROR_STATIC_PESectionExtractor_ENGINE: Gimme a file name!\n";
  exit;
}

if ($arg =~ /^[-\/]s$/i)
{
  $extract_strings++;
  $extract_strings++ if $arg =~ /^[-\/]S$/;
  $arg = shift;
}

my $file = $arg;

if (!open    (FILE, '<'.$file))
{
  print "AHMDS_ERROR_STATIC_PESectionExtractor_ENGINE: Can't open \"$file\"\n";
  exit;
}

binmode (FILE);

my $topdata;

if (!read (FILE, $topdata, 2))
{
  close FILE;
  print "AHMDS_ERROR_STATIC_PESectionExtractor_ENGINE: Can't read \"$file\"\n";
  exit;
}

if ($topdata ne 'MZ')
{
  print "AHMDS_ERROR_STATIC_PESectionExtractor_ENGINE: Not an MZ file\n";
  exit;
}

seek (FILE, 0x3C, 0);
read (FILE, my $o2PE,4);

$o2PE = unpack("I32",$o2PE);
if ($o2PE>16384)
{
  close FILE;
  print "AHMDS_ERROR_STATIC_PESectionExtractor_ENGINE: Offset to PE header (=$o2PE) seems to be quite high\n";
  exit;
}

seek (FILE, $o2PE, 0); read (FILE, my $PEHeader,32); $PEHeader = substr($PEHeader,0,2);

if ($PEHeader ne 'PE')
{
  close FILE;
  print "AHMDS_ERROR_STATIC_PESectionExtractor_ENGINE: Not an PE file\n";
  exit;
}

seek (FILE, $o2PE+4 ,0); read (FILE, my $MachineID,2); $MachineID = unpack("S16",$MachineID);

if ($MachineID != 0x14c && $MachineID != 0x8664)
{
  close FILE;
  print "AHMDS_ERROR_STATIC_PESectionExtractor_ENGINE: PE file type (=$MachineID) not supported \n";
  exit;
}
seek (FILE, $o2PE+6 ,0); read (FILE, my $NumOfSections,2); $NumOfSections = unpack("S16",$NumOfSections);

if ($NumOfSections==0)
{
  close FILE;
  print "AHMDS_ERROR_STATIC_PESectionExtractor_ENGINE: PE file has no sections\n";
  exit;
}

seek (FILE, $o2PE+20,0); read (FILE, my $OptHdrSize,2); $OptHdrSize = unpack("S16",$OptHdrSize);

seek (FILE, $o2PE + 24 + $OptHdrSize , 0); read (FILE, my $sections, $NumOfSections * 40);

for (my $k = 0;$k<$NumOfSections;$k++)
{
   my $sectiondata;
  seek (FILE, $o2PE + 24 + $OptHdrSize + $k*40, 0);
  read (FILE, my $sectionname,8); $sectionname =~ s/[^\x20-\x7F]/ /gs;
  read (FILE, my $vs,4); $vs = unpack("I32",$vs);
  read (FILE, my $vo,4); $vo = unpack("I32",$vo);
  read (FILE, my $fs,4); $fs = unpack("I32",$fs);
  read (FILE, my $fo,4); $fo = unpack("I32",$fo);
  seek (FILE, $o2PE + 24 + $OptHdrSize + $k*40+8+4*4+4+4+4, 0);
  read (FILE, my $sectionflagsbin,4);  $sectionflagsbin = unpack("I32",$sectionflagsbin);
  my $sectionflags = '';
  if ($sectionflagsbin & 0x20000000) { $sectionflags.='X'; }
  if ($sectionflagsbin & 0x80000000) { $sectionflags.='W'; }
  if ($sectionflagsbin & 0x40000000) { $sectionflags.='R'; }
  if ($sectionflagsbin & 0x10000000) { $sectionflags.='S'; }
  if ($sectionflagsbin & 0x00000020) { if ($sectionflags ne '') {$sectionflags.='_'; } $sectionflags.='CODE'; }
  if ($sectionflagsbin & 0x00000040) { if ($sectionflags ne '') {$sectionflags.='_'; } $sectionflags.='IDATA'; }
  if ($sectionflagsbin & 0x00000080) { if ($sectionflags ne '') {$sectionflags.='_'; } $sectionflags.='BSS'; }
  if ($sectionflagsbin & 0x04000000) { if ($sectionflags ne '') {$sectionflags.='_'; } $sectionflags.='NONCACHE'; }
  if ($sectionflagsbin & 0x08000000) { if ($sectionflags ne '') {$sectionflags.='_'; } $sectionflags.='NONPAGE'; }

  $sectionname =~ s/^\s*(.*?)\s*$/$1/;
  if ($sectionname eq '') { $sectionname='SECTION_'.$k; }
  #print STDERR sprintf ("%-8s, vo = %08lX, vs = %08lX, fo = %08lX, fs = %08lX, flags = %08lX, %s", $sectionname, $vo, $vs, $fo, $fs, $sectionflagsbin, $sectionflags)."\n";
  seek (FILE, $fo , 0);
  if ($fs != 0)
  {
    read (FILE, $sectiondata, $fs);
    if ($extract_strings>0)
    {
        next if ($extract_strings==2 && $sectionname =~ /^(\.rsrc|\.reloc)$/i);
        while ($sectiondata =~ /([A-Za-z][ -~]{3,}|([A-Za-z][ -~]\x00){3,})/sg)
        {
           my $string = $1;
           $string =~ s/\x00//gs if $string =~ /([ -~]\x00){3,}/s;
           print "$sectionname\t$string\n";
        }
    }
    else
    {
       #my $sectionfilename = sprintf("%s_%s_%s.dat",$file,$sectionname, $sectionflags);
       my $sectionfilename = sprintf("%s_%d.dat",$file,$k);
       print "$sectionfilename\n";
       open    FSECTION,">$sectionfilename" ;
       binmode FSECTION;
       print   FSECTION $sectiondata;
       close   FSECTION;
    }
  }

}
